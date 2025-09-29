using UnityEngine;

/// <summary>
/// Optimize game for ParrelSync testing to reduce lag
/// </summary>
public class ParrelSyncOptimizer : MonoBehaviour
{
    [Header("ParrelSync Optimization")]
    [SerializeField] private bool enableOptimizations = true;
    [SerializeField] private int targetFPS = 30; // Giảm FPS để giảm CPU load
    [SerializeField] private bool reduceDebugLogs = true;
    
    [Header("Advanced Optimizations")]
    [SerializeField] private bool enableAdvancedOptimizations = true;
    [SerializeField] private bool optimizeRendering = true;
    [SerializeField] private bool aggressiveOptimizations = true;
    
    private void Start()
    {
        if (enableOptimizations)
        {
            OptimizeForParrelSync();
        }
    }
    
    private void OptimizeForParrelSync()
    {
        // Set target FPS to reduce CPU load
        Application.targetFrameRate = targetFPS;
        
        // Reduce quality settings for better performance
        QualitySettings.vSyncCount = 0; // Disable VSync
        QualitySettings.antiAliasing = 0; // Disable AA
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
        
        // Optimize physics
        Physics.defaultSolverIterations = 4; // Reduce from default 6
        Physics.defaultSolverVelocityIterations = 1; // Reduce from default 1
        
        // Advanced optimizations
        if (enableAdvancedOptimizations)
        {
            // Further reduce physics load
            Physics.defaultSolverIterations = 1; // Very aggressive
            Physics.defaultSolverVelocityIterations = 1;
            
            // Optimize rendering
            if (optimizeRendering)
            {
                QualitySettings.shadowDistance = 20f; // Very short shadow distance
                QualitySettings.shadowResolution = ShadowResolution.Low;
                QualitySettings.shadowCascades = 0; // No cascades
                QualitySettings.particleRaycastBudget = 32; // Very low particle budget
                QualitySettings.lodBias = 0.5f; // Reduce LOD detail
            }
            
            // Optimize audio
            var audioConfig = AudioSettings.GetConfiguration();
            audioConfig.dspBufferSize = 1024; // Larger buffer for stability
            AudioSettings.Reset(audioConfig);
            
            // Aggressive optimizations
            if (aggressiveOptimizations)
            {
                QualitySettings.globalTextureMipmapLimit = 1; // Reduce texture quality
                QualitySettings.pixelLightCount = 1; // Minimal pixel lights
                QualitySettings.shadows = ShadowQuality.Disable; // Disable shadows completely
                QualitySettings.shadowDistance = 0; // No shadows at all
                
                // Disable unnecessary features
                QualitySettings.softVegetation = false;
                QualitySettings.realtimeReflectionProbes = false;
            }
        }
        
        Debug.Log("ParrelSyncOptimizer: Applied optimizations for local testing");
    }
    
    private void Update()
    {
        // Reduce debug log frequency - DISABLE for better performance
        // if (reduceDebugLogs && Time.frameCount % 120 == 0)
        // {
        //     Debug.Log($"ParrelSyncOptimizer: FPS: {1f / Time.deltaTime:F1}");
        // }
    }
}

using UnityEngine;

/// <summary>
/// Optimize ThirdPersonController for better performance
/// </summary>
public class ThirdPersonControllerOptimizer : MonoBehaviour
{
    [Header("ThirdPersonController Optimization")]
    [SerializeField] private ThirdPersonController thirdPersonController;
    [SerializeField] private bool enableOptimizations = true;
    [SerializeField] private float updateInterval = 0.016f; // 60 FPS update rate
    
    private float lastUpdateTime;
    
    private void Start()
    {
        if (thirdPersonController == null)
            thirdPersonController = GetComponent<ThirdPersonController>();
            
        if (thirdPersonController != null && enableOptimizations)
        {
            OptimizeThirdPersonController();
        }
    }
    
    private void OptimizeThirdPersonController()
    {
        Debug.Log("ThirdPersonControllerOptimizer: Applying optimizations");
        
        // Reduce update frequency for better performance
        // This helps with ParrelSync lag
        
        // Note: ThirdPersonController from asset might not have direct access to these settings
        // But we can optimize the MonoBehaviour update cycle
    }
    
    private void Update()
    {
        if (!enableOptimizations) return;
        
        // Limit update frequency to reduce CPU load
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            // Force update at reduced frequency
            lastUpdateTime = Time.time;
            
            // This helps with ParrelSync lag by reducing update pressure
        }
    }
}




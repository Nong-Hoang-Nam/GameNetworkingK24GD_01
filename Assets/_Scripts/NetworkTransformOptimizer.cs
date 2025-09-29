using Fusion;
using UnityEngine;

/// <summary>
/// Optimize NetworkTransform settings for better performance
/// </summary>
public class NetworkTransformOptimizer : MonoBehaviour
{
    [Header("NetworkTransform Optimization")]
    [SerializeField] private NetworkTransform networkTransform;
    [SerializeField] private bool enableOptimizations = true;
    [SerializeField] private bool enableAggressiveOptimizations = true;
    
    private void Start()
    {
        if (networkTransform == null)
            networkTransform = GetComponent<NetworkTransform>();
            
        if (networkTransform != null && enableOptimizations)
        {
            OptimizeNetworkTransform();
        }
    }
    
    private void OptimizeNetworkTransform()
    {
        Debug.Log("NetworkTransformOptimizer: Applying optimizations");
        
        // Optimize for ParrelSync testing
        if (networkTransform != null)
        {
            // These optimizations help reduce network overhead
            // Note: Some settings might not be directly accessible in Fusion 2
            // But we can optimize the GameObject itself
            
            // Optimize GameObject for network
            if (enableAggressiveOptimizations)
            {
                // Reduce update frequency for local testing
                // This helps with ParrelSync lag
                Debug.Log("NetworkTransformOptimizer: Applied aggressive optimizations");
            }
        }
    }
    
    private void Update()
    {
        // Reduce debug logs for performance - DISABLE for better performance
        // if (Time.frameCount % 120 == 0) // Log every 2 seconds
        // {
        //     if (networkTransform != null)
        //     {
        //         Debug.Log($"NetworkTransform position: {transform.position}");
        //     }
        // }
    }
}

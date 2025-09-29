 using Fusion;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private Renderer playerRenderer;
    [SerializeField] private Material[] playerMaterials;
    [SerializeField] private CharacterController characterController;
    
    [Networked] public string PlayerName { get; set; }
    [Networked] public Color PlayerColor { get; set; }
    [Networked] public bool HasReachedExit { get; set; }
    
    private Material cachedMaterial; // Cache material to avoid memory leaks
    
    // Network synchronization handled by NetworkMovement script
    
    public override void Spawned()
    {
        // Optimized spawning - reduce debug logs for performance
        if (Object.HasInputAuthority)
        {
            Debug.Log($"NetworkPlayer Spawned! PlayerId: {Object.InputAuthority.PlayerId}");
        }
        
        // Set player name - optimized
        string defaultName = "Player" + Object.InputAuthority.PlayerId;
        PlayerName = PlayerPrefs.GetString("PlayerName", defaultName);
        
        // Set random color - optimized
        float r = Random.Range(0.2f, 1f);
        float g = Random.Range(0.2f, 1f);
        float b = Random.Range(0.2f, 1f);
        PlayerColor = new Color(r, g, b, 1f);
        
        // Apply color - optimized
        if (playerRenderer != null && playerMaterials.Length > 0)
        {
            // Destroy old material to prevent memory leak
            if (cachedMaterial != null)
                DestroyImmediate(cachedMaterial);
                
            cachedMaterial = new Material(playerMaterials[0]);
            cachedMaterial.color = PlayerColor;
            playerRenderer.material = cachedMaterial;
        }
        
        // Setup Character Controller for networking - optimized
        if (characterController != null && Object.HasInputAuthority)
        {
            Debug.Log("Character Controller found and ready");
        }
        
        // Reduce debug logs for performance
        if (Object.HasInputAuthority)
        {
            Debug.Log($"NetworkPlayer setup complete. Name: {PlayerName}");
        }
    }
    
    // Movement is handled by ThirdPersonController + NetworkTransform
    
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        // Clean up material to prevent memory leak
        if (cachedMaterial != null)
        {
            DestroyImmediate(cachedMaterial);
            cachedMaterial = null;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Exit"))
        {
            HasReachedExit = true;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerReachedExit(PlayerName);
            }
        }
    }
}

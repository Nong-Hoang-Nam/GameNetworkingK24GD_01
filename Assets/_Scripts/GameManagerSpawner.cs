using Fusion;
using UnityEngine;

/// <summary>
/// Spawn GameManager as NetworkObject
/// </summary>
public class GameManagerSpawner : SimulationBehaviour
{
    [SerializeField] private NetworkPrefabRef gameManagerPrefab;
    private bool hasSpawnedGameManager = false;
    private bool hasCheckedConnection = false;
    
    private void Update()
    {
        // Debug log every 60 frames to see if Update is being called
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"GameManagerSpawner: Update called - Runner: {Runner != null}, hasSpawned: {hasSpawnedGameManager}, hasChecked: {hasCheckedConnection}");
        }
        
        // Check if Runner is ready and we haven't spawned yet
        if (Runner != null && !hasSpawnedGameManager && !hasCheckedConnection)
        {
            Debug.Log("GameManagerSpawner: Runner is ready, checking connection...");
            hasCheckedConnection = true;
            
            if (Runner.IsSharedModeMasterClient)
            {
                Debug.Log("GameManagerSpawner: Master client detected, spawning GameManager...");
                SpawnGameManager();
                hasSpawnedGameManager = true;
            }
            else
            {
                Debug.Log("GameManagerSpawner: Not master client, waiting...");
            }
        }
    }
    
    private void SpawnGameManager()
    {
        Debug.Log("GameManagerSpawner: SpawnGameManager() called");
        
        if (gameManagerPrefab == null)
        {
            Debug.LogError("GameManagerSpawner: GameManager prefab not assigned!");
            return;
        }
        
        Debug.Log("GameManagerSpawner: GameManager prefab found, attempting to spawn...");
        
        // Spawn GameManager at origin
        var gameManager = Runner.Spawn(gameManagerPrefab, Vector3.zero, Quaternion.identity);
        
        if (gameManager != null)
        {
            Debug.Log("GameManagerSpawner: GameManager spawned successfully!");
        }
        else
        {
            Debug.LogWarning("GameManagerSpawner: Spawn returned null, will retry next frame");
            hasSpawnedGameManager = false;
            hasCheckedConnection = false;
        }
    }
}

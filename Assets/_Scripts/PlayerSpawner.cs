 using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private NetworkPrefabRef playerPrefab;
    
    // Hardcoded spawn positions for GameScene
    private Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(3.75f, 1f, 7f),    // SpawnPoint1 position
        new Vector3(3.91f, 1f, 9f)     // SpawnPoint2 position
    };
    
    private void Start()
    {
        // Reduced debug logs for performance
        if (Runner != null && Runner.IsSharedModeMasterClient)
        {
            Debug.Log("PlayerSpawner: Start() called");
        }
    }
    
    public void PlayerJoined(PlayerRef player)
    {
        // Only spawn for the local player to avoid duplicates
        if (player == Runner.LocalPlayer)
        {
            Debug.Log($"PlayerSpawner: PlayerJoined called for local player {player.PlayerId}");
            SpawnPlayer(player);
        }
    }
    
    private void SpawnPlayer(PlayerRef player)
    {
        if (Runner == null || playerPrefab == null)
        {
            Debug.LogError("PlayerSpawner: Missing required components!");
            return;
        }
        
        // Get spawn position based on player ID
        int spawnIndex = player.PlayerId % spawnPositions.Length;
        Vector3 spawnPosition = spawnPositions[spawnIndex];
        
        // Reduced debug logs for performance
        Debug.Log($"Spawning player {player.PlayerId} at position: {spawnPosition}");
        
        // Spawn the player
        var spawnedPlayer = Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
        
        if (spawnedPlayer != null)
        {
            Debug.Log($"Player {player.PlayerId} spawned successfully!");
        }
        else
        {
            Debug.LogError($"Failed to spawn player {player.PlayerId}!");
        }
    }
}

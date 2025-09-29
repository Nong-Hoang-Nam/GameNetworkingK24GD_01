 using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private int spawnIndex = 0;
    
    public int SpawnIndex => spawnIndex;
    
    private void OnDrawGizmos()
    {
        // Draw spawn point in scene view
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1f);
        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, Vector3.one);
    }
}

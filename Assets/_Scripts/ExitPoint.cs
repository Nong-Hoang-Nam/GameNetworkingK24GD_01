 using UnityEngine;

public class ExitPoint : MonoBehaviour
{
    private void Start()
    {
        // Ensure this object has a trigger collider
        if (GetComponent<Collider>() == null)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
        }
        else
        {
            GetComponent<Collider>().isTrigger = true;
        }
        
        // Add tag if not present
        if (!gameObject.CompareTag("Exit"))
        {
            gameObject.tag = "Exit";
        }
    }
    
    private void OnDrawGizmos()
    {
        // Draw exit point in scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, Vector3.one);
    }
}

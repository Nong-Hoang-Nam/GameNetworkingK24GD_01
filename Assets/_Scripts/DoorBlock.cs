 using UnityEngine;

public class DoorBlock : MonoBehaviour
{
    [SerializeField] private Collider doorCollider;
    
    private bool isOpen = false;
    
    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            
            // Disable collider
            if (doorCollider != null)
            {
                doorCollider.enabled = false;
            }
            
            // Hide door
            gameObject.SetActive(false);
        }
    }
    
    public void CloseDoor()
    {
        if (isOpen)
        {
            isOpen = false;
            
            // Enable collider
            if (doorCollider != null)
            {
                doorCollider.enabled = true;
            }
            
            // Show door
            gameObject.SetActive(true);
        }
    }
}

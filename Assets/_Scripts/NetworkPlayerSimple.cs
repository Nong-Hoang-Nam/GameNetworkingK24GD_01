using Fusion;
using UnityEngine;

/// <summary>
/// Simple NetworkPlayer with NetworkTransform (no custom movement sync)
/// Uses NetworkTransform for automatic position/rotation sync
/// </summary>
public class NetworkPlayerSimple : NetworkBehaviour
{
    [Header("Player Components")]
    [SerializeField] private Renderer playerRenderer;
    [SerializeField] private Material[] playerMaterials;
    private Renderer[] allRenderers;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private ThirdPersonController thirdPersonController;
    
    [Header("Network Properties")]
    [Networked] public string PlayerName { get; set; }
    [Networked] public Color PlayerColor { get; set; }
    [Networked] public bool HasReachedExit { get; set; }
    
    private Material cachedMaterial;
    private bool colorApplied;
    
    public override void Spawned()
    {
        // Get components
        characterController = GetComponent<CharacterController>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        playerRenderer = GetComponent<Renderer>();
        allRenderers = GetComponentsInChildren<Renderer>(true);
        
        // Set player name
        string defaultName = "Player" + Object.InputAuthority.PlayerId;
        PlayerName = PlayerPrefs.GetString("PlayerName", defaultName);
        
        // Set random color only by StateAuthority, others will receive via network
        if (Object.HasStateAuthority)
        {
            float r = Random.Range(0.2f, 1f);
            float g = Random.Range(0.2f, 1f);
            float b = Random.Range(0.2f, 1f);
            PlayerColor = new Color(r, g, b, 1f);
        }
        
        // Apply color
        ApplyColorIfReady();
        
        // Setup components for networking
        if (Object.HasInputAuthority)
        {
            // Enable ThirdPersonController for local player
            if (thirdPersonController != null)
            {
                thirdPersonController.enabled = true;
                Debug.Log("ThirdPersonController enabled for local player");
            }

                // Camera zoom đã chuyển sang CameraController
        }
        else
        {
            // Disable ThirdPersonController for remote players
            if (thirdPersonController != null)
            {
                thirdPersonController.enabled = false;
                Debug.Log("ThirdPersonController disabled for remote player");
            }
        }
        
        // Fix: If spawned at (0,0,0), set to correct spawn position
        if (transform.position == Vector3.zero)
        {
            // Get spawn position based on player ID
            Vector3[] spawnPositions = new Vector3[]
            {
                new Vector3(3.75f, 1f, 7f),    // SpawnPoint1 position
                new Vector3(3.91f, 1f, 9f)     // SpawnPoint2 position
            };
            
            int spawnIndex = Object.InputAuthority.PlayerId % spawnPositions.Length;
            Vector3 correctPosition = spawnPositions[spawnIndex];
            
            transform.position = correctPosition;
            
            Debug.Log($"Fixed Player {Object.InputAuthority.PlayerId} position to: {correctPosition}");
        }
        
        Debug.Log($"NetworkPlayerSimple setup complete. Name: {PlayerName}");
    }
    
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
        if (other.CompareTag("Exit") || other.name == "ExitPoint")
        {
            HasReachedExit = true;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerReachedExit(PlayerName);
            }
        }
    }

    private void LateUpdate()
    {
        // When network delivers PlayerColor to remotes, apply once
        if (!colorApplied && PlayerColor.a > 0f)
        {
            ApplyColorIfReady();
        }
    }

    private void ApplyColorIfReady()
    {
        if (PlayerColor.a <= 0f)
            return;

        // Prefer MaterialPropertyBlock to avoid material instances
        var mpb = new MaterialPropertyBlock();
        mpb.SetColor("_Color", PlayerColor);        // Built-in/Standard
        mpb.SetColor("_BaseColor", PlayerColor);    // URP Lit

        if (allRenderers != null && allRenderers.Length > 0)
        {
            // Ưu tiên chỉ tô màu cho renderer có tên "helmet.002" nếu tồn tại
            bool appliedToHelmet = false;
            foreach (var rend in allRenderers)
            {
                if (!rend) continue;
                if (rend.transform.name == "helmet.002")
                {
                    int mats = rend.sharedMaterials != null ? rend.sharedMaterials.Length : 1;
                    for (int i = 0; i < mats; i++)
                    {
                        rend.SetPropertyBlock(mpb, i);
                    }
                    appliedToHelmet = true;
                }
            }

            // Nếu không tìm thấy helmet.002, áp dụng cho tất cả renderers con
            if (!appliedToHelmet)
            {
                foreach (var rend in allRenderers)
                {
                    if (!rend) continue;
                    int mats = rend.sharedMaterials != null ? rend.sharedMaterials.Length : 1;
                    for (int i = 0; i < mats; i++)
                    {
                        rend.SetPropertyBlock(mpb, i);
                    }
                }
            }
            colorApplied = true;
            return;
        }

        // Fallback to single renderer + instance
        if (playerRenderer != null && playerMaterials.Length > 0)
        {
            if (cachedMaterial != null)
                DestroyImmediate(cachedMaterial);
            cachedMaterial = new Material(playerMaterials[0]);
            cachedMaterial.color = PlayerColor;
            playerRenderer.material = cachedMaterial;
            colorApplied = true;
        }
    }
        
}

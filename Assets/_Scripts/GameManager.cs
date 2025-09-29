 using Fusion;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private GameObject doorBlock;
    [SerializeField] private TextMeshProUGUI waitingText;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Button quitButton;
    
    [Networked] public bool GameStarted { get; set; }
    [Networked] public bool GameEnded { get; set; }
    [Networked] public string WinnerName { get; set; }
    
    public static GameManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public override void Spawned()
    {
        // Find UI elements automatically
        FindUIElements();
        
        // Only the first client (master) manages game state
        if (Object.HasStateAuthority)
        {
            GameStarted = false;
            GameEnded = false;
        }
        
        UpdateUI();
        
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
        }
    }
    
    private void FindUIElements()
    {
        // Note: GameObject.Find does NOT find inactive objects.
        // Use includeInactive=true searches so UI can be pre-hidden in the scene.

        if (doorBlock == null)
        {
            var allTransforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            var door = System.Array.Find(allTransforms, t => t.name == "DoorBlock");
            if (door != null) doorBlock = door.gameObject;
        }

        if (waitingText == null)
        {
            var allTexts = UnityEngine.Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            waitingText = System.Array.Find(allTexts, t => t.name == "WaitingText");
        }

        if (winnerText == null)
        {
            var allTexts = UnityEngine.Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            winnerText = System.Array.Find(allTexts, t => t.name == "WinnerText");
        }

        if (winPanel == null)
        {
            var allTransforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            var panel = System.Array.Find(allTransforms, t => t.name == "WinPanel");
            if (panel != null) winPanel = panel.gameObject;
        }
        
        if (quitButton == null)
        {
            var allButtons = UnityEngine.Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            quitButton = System.Array.Find(allButtons, b => b.name == "QuitButton");
        }

        Debug.Log("GameManager: Found UI elements automatically (including inactive)");
    }
    
    public override void FixedUpdateNetwork()
    {
        // Luôn cập nhật UI cho mọi client
        UpdateUI();
    }
    
    public void OnPlayerReachedExit(string playerName)
    {
        if (!GameEnded)
        {
            if (Object.HasStateAuthority)
            {
                GameEnded = true;
                WinnerName = playerName;
                RpcShowWin(playerName);
            }
            else
            {
                RpcReportWin(playerName);
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RpcReportWin(string winner)
    {
        if (!GameEnded)
        {
            GameEnded = true;
            WinnerName = winner;
            RpcShowWin(winner);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcShowWin(string winner)
    {
        WinnerName = winner;
        GameEnded = true;
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        if (waitingText != null)
        {
            if (!GameStarted)
            {
                var count = Runner.ActivePlayers.Count();
                if (count < 2)
                {
                    waitingText.text = $"Waiting for players... ({count}/2)";
                    waitingText.gameObject.SetActive(true);
                }
                else
                {
                    waitingText.text = "Press F to start the game!";
                    waitingText.gameObject.SetActive(true);
                }
            }
            else
            {
                // Game đã bắt đầu -> ẩn text
                waitingText.gameObject.SetActive(false);
            }
        }
        
        if (doorBlock != null)
        {
            doorBlock.SetActive(!GameStarted);
        }
        
        if (winPanel != null)
        {
            winPanel.SetActive(GameEnded);
            if (GameEnded)
            {
                SetCursor(true);
            }
        }
        
        if (winnerText != null && GameEnded)
        {
            winnerText.text = $"Winner: {WinnerName}";
        }
    }
    
    public void StartGame()
    {
        if (GameStarted && !GameEnded)
        {
            if (doorBlock != null)
            {
                doorBlock.SetActive(false);
            }
            if (waitingText != null)
            {
                waitingText.gameObject.SetActive(false);
            }
        }
    }
    
    public void QuitGame()
    {
        // Nếu có Main Menu, bạn có thể thay bằng LoadScene(Index/Name)
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    
    private void Update()
    {
        // Bắt đầu game khi đủ 2 người và người chơi nhấn F
        if (Object != null && Object.IsValid && !GameEnded)
        {
            if (!GameStarted && Runner.ActivePlayers.Count() >= 2 && Input.GetKeyDown(KeyCode.F))
            {
                if (Object.HasStateAuthority)
                {
                    GameStarted = true;
                    RpcApplyStart();
                }
                else
                {
                    RpcRequestStart();
                }
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RpcRequestStart()
    {
        if (!GameEnded && !GameStarted)
        {
            GameStarted = true;
            RpcApplyStart();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcApplyStart()
    {
        // áp dụng ngay trên mọi client tránh trễ đồng bộ
        ApplyGameStartedState();
    }

    // Áp dụng trạng thái khi GameStarted thay đổi (được gọi sau khi set)
    private void ApplyGameStartedState()
    {
        // Ẩn/hiện UI và cửa theo trạng thái mạng (chạy trên mọi client)
        if (doorBlock != null)
            doorBlock.SetActive(!GameStarted);

        if (waitingText != null)
            waitingText.gameObject.SetActive(!GameStarted);

        // Khi game bắt đầu: ẩn con trỏ để chơi; khi chưa bắt đầu: hiện con trỏ
        SetCursor(!GameStarted);
    }

    private void SetCursor(bool show)
    {
        Cursor.visible = show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
    }
}


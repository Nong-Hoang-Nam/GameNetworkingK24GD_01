 using Fusion;
using UnityEngine;
using System.Linq;

public class NetworkManager : NetworkBehaviour
{
    public static NetworkManager Instance { get; private set; }
    
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
    
    public int GetPlayerCount()
    {
        return Runner != null ? Runner.ActivePlayers.Count() : 0;
    }
}

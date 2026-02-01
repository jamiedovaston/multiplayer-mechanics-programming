using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSessionManager : NetworkBehaviour
{
    [SerializeField] private GameObject m_Player;
    [SerializeField] private Transform m_SpawnPosition;

    private PlayerType m_P1, m_P2;
    private ulong m_p1ID, m_p2ID;

    public override void OnNetworkSpawn()
    {
        NetworkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;

        if (!IsServer) return;

        NetworkManager.OnServerStarted += OnServerStarted;

        NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;

        PlayerLobby.OnStartGame += StartGame;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;

        if (!IsServer) return;

        NetworkManager.OnServerStarted -= OnServerStarted;

        NetworkManager.SceneManager.OnSceneEvent -= OnSceneEvent;

        PlayerLobby.OnStartGame -= StartGame;
    }

    private void OnClientDisconnectCallback(ulong clientID)
    {
        if (IsServer) return;

        if (clientID != NetworkManager.LocalClientId) return;

        if(SceneManager.GetSceneByName("Game").isLoaded)
            SceneManager.UnloadSceneAsync("Game");
    }

    private void OnServerStarted()
    {
        NetworkManager.SceneManager.LoadScene("Lobby", LoadSceneMode.Additive);
    }

    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType == SceneEventType.Unload)
        {
            if (sceneEvent.SceneName != "Game")
                return;
            NetworkManager.Shutdown();
        }

        if (!IsServer) return;

        if (sceneEvent.SceneEventType != SceneEventType.LoadComplete)
            return;

        if (sceneEvent.SceneName != "Game")
            return;

        NetworkManager.SceneManager.UnloadScene(SceneManager.GetSceneByName("Lobby"));
        Spawn(sceneEvent.ClientId);
    }

    private void StartGame(ulong p1ID, PlayerType p1, ulong p2ID, PlayerType p2)
    {
        m_p1ID = p1ID;
        m_p2ID = p2ID;
        m_P1 = p1;
        m_P2 = p2;

        NetworkManager.SceneManager.LoadScene("Game", LoadSceneMode.Additive);
    }

    public void Spawn(ulong id)
    {
        Vector3 randomPos = (Vector3.right * Random.Range(-3.0f, 3.0f)) + (Vector3.forward * Random.Range(-3.0f, 3.0f));
        GameObject p = Instantiate(m_Player, m_SpawnPosition.position + randomPos, Quaternion.identity);
        p.GetComponent<NetworkObject>().SpawnAsPlayerObject(id);

        PlayerType type = PlayerType.Bomb;
        if (m_p1ID == id) type = m_P1;
        if (m_p2ID == id) type = m_P2;
        p.GetComponent<NetworkedPlayerController>().InitialiseRpc(type);
    }
}

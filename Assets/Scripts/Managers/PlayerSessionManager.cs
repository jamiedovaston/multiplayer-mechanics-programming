using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSessionManager : NetworkBehaviour
{
    [SerializeField] private GameObject m_Player;
    [SerializeField] private Transform m_SpawnPosition;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        NetworkManager.OnClientConnectedCallback += OnClientConnectCallback;
        NetworkManager.OnServerStarted += OnServerStarted;

        NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;

        PlayerLobby.OnStartGame += StartGame;
    }


    public override void OnNetworkDespawn()
    {
        SceneManager.UnloadSceneAsync("Lobby");

        if (!IsServer) return;

        NetworkManager.OnClientConnectedCallback -= OnClientConnectCallback;
        NetworkManager.OnServerStarted -= OnServerStarted;

        NetworkManager.SceneManager.OnSceneEvent -= OnSceneEvent;

        PlayerLobby.OnStartGame -= StartGame;
    }

    private void OnServerStarted()
    {
        NetworkManager.SceneManager.LoadScene("Lobby", LoadSceneMode.Additive);
    }

    public void OnClientConnectCallback(ulong clientID)
    {
        if (!IsServer) return;

        Spawn(clientID);
    }

    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        foreach (ulong clientID in NetworkManager.ConnectedClientsIds)
        {
            SpawnRpc(clientID);
        }

        if (sceneEvent.SceneName == "Game")
        {
            NetworkManager.SceneManager.UnloadScene(SceneManager.GetSceneByName("Lobby"));
        }
    }

    private void StartGame()
    {
        /// FIIXXXXXXXXXXXXXXXXXXXX
        NetworkManager.SceneManager.LoadScene("Game", LoadSceneMode.Additive);
    }

    [Rpc(SendTo.Server)]
    public void SpawnRpc(ulong id)
    {
        Vector3 randomPos = (Vector3.right * Random.Range(-3.0f, 3.0f)) + (Vector3.forward * Random.Range(-3.0f, 3.0f));
        GameObject p = Instantiate(m_Player, m_SpawnPosition.position + randomPos, Quaternion.identity);
        p.GetComponent<NetworkObject>().SpawnAsPlayerObject(id);
        p.GetComponent<NetworkedPlayerController>().InitialiseRpc(false); //TEMP
    }
}

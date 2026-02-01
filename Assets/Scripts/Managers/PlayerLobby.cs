using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public enum PlayerType
{
    Bat = 0,
    Bomb = 1
}

public class PlayerLobby : NetworkBehaviour
{
    public static Action<ulong, PlayerType, ulong, PlayerType> OnStartGame;

    public TMP_Text m_LobbyCodeText;

    // Player 1
    public GameObject m_Player1, m_Player1BatModel, m_Player1BombModel;
    public GameObject m_Player2, m_Player2BatModel, m_Player2BombModel;
    public SkinnedMeshRenderer m_Player1CharacterMesh, m_Player2CharacterMesh;
    public Animator m_Player1Anim, m_Player2Anim;
    public TMP_Text m_Player1Text, m_Player2Text;
    private ulong m_Player1ID, m_Player2ID;
    private PlayerType m_Player1Type, m_Player2Type;

    public GameObject m_SwapButton;

    public GameObject m_ReadyButton, m_StartGameButton, m_UnreadyButton;

    private List<ulong> m_ReadyClients = new List<ulong>();

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;

        m_LobbyCodeText.text = $"Code : { RelayManager.Instance.joinCode }";

        m_ReadyButton.SetActive(true);
        m_UnreadyButton.SetActive(false);
        m_StartGameButton.SetActive(false);

        m_SwapButton.SetActive(IsHost);

        m_Player1ID = NetworkManager.LocalClientId;

        CheckPlayers();

        if (IsServer)
            ApplyPlayerSettingsRpc(PlayerType.Bat, PlayerType.Bomb);
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
    }

    public void OnClientConnectCallback(ulong clientID)
    {
        CheckPlayers();
        m_Player2ID = clientID;

        if (IsServer) {
            ApplyPlayerSettingsRpc(m_Player1Type, m_Player2Type);
            return;
        }

        m_LobbyCodeText.text = $"Code : { RelayManager.Instance.joinCode }";
    }

    public void ReadyUp()
    {
        m_ReadyButton.SetActive(false);
        m_UnreadyButton.SetActive(true);
        ReadyUpRpc(NetworkManager.LocalClient.ClientId);
    }

    public void UnreadyUp()
    {
        m_ReadyButton.SetActive(true);
        m_UnreadyButton.SetActive(false);
        UnreadyUpRpc(NetworkManager.LocalClient.ClientId);
    }

    public void StartGame()
    {
        m_ReadyButton.SetActive(false);
        m_UnreadyButton.SetActive(false);
        m_StartGameButton.SetActive(false);

        OnStartGame?.Invoke(m_Player1ID, m_Player1Type, m_Player2ID, m_Player2Type);
    }

    [Rpc(SendTo.Server)]
    public void ReadyUpRpc(ulong clientID)
    {
        if (!m_ReadyClients.Contains(clientID))
            m_ReadyClients.Add(clientID);

        if (m_ReadyClients.Count == NetworkManager.ConnectedClientsList.Count && NetworkManager.ConnectedClientsList.Count >= 2)
            m_StartGameButton.SetActive(true);
    }

    [Rpc(SendTo.Server)]
    public void UnreadyUpRpc(ulong clientID)
    {
        if(m_ReadyClients.Contains(clientID))
            m_ReadyClients.Remove(clientID);

        if (m_ReadyClients.Count != NetworkManager.ConnectedClientsList.Count || NetworkManager.ConnectedClientsList.Count < 2)
        {
            m_StartGameButton.SetActive(false);
        }
    }

    private void OnClientDisconnectCallback(ulong clientID)
    {
        if(clientID == NetworkManager.LocalClientId) UnreadyUpRpc(NetworkManager.LocalClientId);

        if (m_Player2ID == clientID) m_Player2ID = ulong.MinValue;
        m_Player2ID = clientID;
        m_ReadyClients.Remove(clientID);
        CheckPlayers();
    }

    public void Swap()
    {
        if (!IsServer) return;

        m_Player1Type = (m_Player1Type == PlayerType.Bat
            ? PlayerType.Bomb
            : PlayerType.Bat);

        m_Player2Type = (m_Player1Type == PlayerType.Bat
            ? PlayerType.Bomb
            : PlayerType.Bat);

        ApplyPlayerSettingsRpc(m_Player1Type, m_Player2Type);
    }

    public void CheckPlayers()
    {
        if (NetworkManager.ConnectedClientsList.Count == 1)
        {
            m_Player2.SetActive(false);
        }
        if (NetworkManager.ConnectedClientsList.Count >= 2)
        {
            m_Player2.SetActive(true);
        }
    }

    public void Leave() =>
        NetworkManager.Shutdown();

    [Rpc(SendTo.Everyone)]
    public void ApplyPlayerSettingsRpc(PlayerType p1, PlayerType p2)
    {
        m_Player1Type = p1;
        m_Player2Type = p2;

        m_Player1Text.text = (p1 == PlayerType.Bat ? "Bat" : "Bomb");
        m_Player2Text.text = (p2 == PlayerType.Bat ? "Bat" : "Bomb");

        m_Player1Anim.SetInteger("Weapon", (int)m_Player1Type);
        m_Player2Anim.SetInteger("Weapon", (int)m_Player2Type);

        m_Player1CharacterMesh.material.color = m_Player1Type == PlayerType.Bat ? Color.blue : Color.red;
        m_Player2CharacterMesh.material.color = m_Player2Type == PlayerType.Bat ? Color.blue : Color.red;

        m_Player1Anim.SetTrigger("ChangeWeapon");
        m_Player2Anim.SetTrigger("ChangeWeapon");

        m_Player1BatModel.SetActive(p1 == PlayerType.Bat);
        m_Player1BombModel.SetActive(p1 != PlayerType.Bat);
        m_Player2BatModel.SetActive(p2 == PlayerType.Bat);
        m_Player2BombModel.SetActive(p2 != PlayerType.Bat);
    }
}


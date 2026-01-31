using Mono.Cecil.Cil;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerLobby : NetworkBehaviour
{
    public static Action OnStartGame;

    public TMP_Text m_LobbyCodeText;

    public enum PlayerType
    {
        Bat = 0,
        Bomb = 1
    }

    // Player 1
    public GameObject m_Player1, m_Player1BatModel, m_Player1BombModel;
    public GameObject m_Player2, m_Player2BatModel, m_Player2BombModel;
    public Animator m_Player1Anim, m_Player2Anim;
    public TMP_Text m_Player1Text, m_Player2Text;
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

        OnStartGame?.Invoke();
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

        m_Player1Anim.SetTrigger("ChangeWeapon");
        m_Player2Anim.SetTrigger("ChangeWeapon");

        m_Player1BatModel.SetActive(p1 == PlayerType.Bat);
        m_Player1BombModel.SetActive(p1 != PlayerType.Bat);
        m_Player2BatModel.SetActive(p2 == PlayerType.Bat);
        m_Player2BombModel.SetActive(p2 != PlayerType.Bat);
    }
}


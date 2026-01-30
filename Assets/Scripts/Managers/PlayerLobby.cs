using Mono.Cecil.Cil;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerLobby : NetworkBehaviour
{
    public TMP_Text m_LobbyCodeText;

    public enum PlayerType
    {
        Bat = 0,
        Bomb = 1
    }

    // Player 1
    public GameObject m_Player1Model, m_Player2Model;
    public TMP_Text m_Player1Text, m_Player2Text;
    private PlayerType m_Player1Type, m_Player2Type;


    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectCallback;

        m_LobbyCodeText.text = $"Code : {RelayManager.Instance.joinCode}";
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectCallback;
    }

    public void OnClientConnectCallback(ulong clientID)
    {
        if (IsServer) return;
        ApplyPlayerSettingsRpc(m_Player1Type, m_Player2Type);

        m_LobbyCodeText.text = $"Code : {RelayManager.Instance.joinCode}";
        
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

    [Rpc(SendTo.Everyone)]
    public void ApplyPlayerSettingsRpc(PlayerType p1, PlayerType p2)
    {
        m_Player1Type = p1;
        m_Player2Type = p2;

        m_Player1Text.text = (p1 == PlayerType.Bat ? "Bat" : "Bomb");
        m_Player2Text.text = (p2 == PlayerType.Bat ? "Bat" : "Bomb");
    }
}


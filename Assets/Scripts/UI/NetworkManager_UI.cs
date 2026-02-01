using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager_UI : NetworkBehaviour
{
    // MISC
    [Header("Misc")]
    public Canvas m_ConnectingPanel;

    // HOME
    [Header("Home")]
    public Canvas m_HomePanel;
    public Button m_HostButton, m_ClientButton;
    public TMP_InputField m_CodeInputField;

    // GAME
    [Header("Game")]
    public Canvas m_GamePanel;
    public Button m_DisconnectButton;

    private void Start()
    {
        m_HostButton.onClick.AddListener(async () =>
        {
            m_HomePanel.enabled = false;
            m_ConnectingPanel.enabled = true;
            bool success = await RelayManager.Instance.StartHost();
            m_ConnectingPanel.enabled = false;

            SuccessfullyConnectedToLobby(success);
        });

        m_ClientButton.onClick.AddListener(async () =>
        {
            m_HomePanel.enabled = false;
            m_ConnectingPanel.enabled = true;
            bool success = await RelayManager.Instance.StartClient(m_CodeInputField.text);
            m_ConnectingPanel.enabled = false;

            SuccessfullyConnectedToLobby(success);
        });

        m_DisconnectButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
        });
    }

    private void SuccessfullyConnectedToLobby(bool success)
    {
        m_HomePanel.enabled = !success;
    }

    public override void OnNetworkSpawn()
    {
        m_ConnectingPanel.enabled = false;
        m_HomePanel.enabled = false;
    }

    public override void OnNetworkDespawn()
    {
        m_HomePanel.enabled = true;
    }
}

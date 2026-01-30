using System.Threading.Tasks;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager_UI : NetworkBehaviour
{
    // HOME
    public Canvas m_DisconnectedPanel;
    public Button m_HostButton, m_ClientButton, m_ServerButton;

    // LOBBY
    public Canvas m_ConnectedPanel;
    public Button m_DisconnectButton;

    public string tempCode;

    private void Start()
    {
        m_HostButton.onClick.AddListener(async () =>
        {
            string code = await NetworkServices.StartHostWithRelay(2, "udp");

            Debug.Log($"Code: { code }");
        });

        m_ServerButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });

        m_ClientButton.onClick.AddListener(async () =>
        {
            await NetworkServices.StartClientWithRelay(tempCode, "udp");
        });

        m_DisconnectButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
        });
    }

    public override void OnNetworkSpawn()
    {
        m_ConnectedPanel.enabled = true;
        m_DisconnectedPanel.enabled = false;
    }

    public override void OnNetworkDespawn()
    {
        m_ConnectedPanel.enabled = false;
        m_DisconnectedPanel.enabled = true;
    }
}

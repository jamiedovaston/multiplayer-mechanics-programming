using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager_UI : MonoBehaviour
{
    public Button m_HostButton, m_ClientButton, m_ServerButton;

    private void Start()
    {
        m_HostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });

        m_ServerButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });

        m_ClientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }
}

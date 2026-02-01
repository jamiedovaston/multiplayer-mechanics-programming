using Unity.Netcode;
using UnityEngine;

public class VoidScript : NetworkBehaviour
{
    [SerializeField] private Transform m_TeleportPosition;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<NetworkedPlayerController>().TeleportRpc(m_TeleportPosition.position);
        }
    }
}

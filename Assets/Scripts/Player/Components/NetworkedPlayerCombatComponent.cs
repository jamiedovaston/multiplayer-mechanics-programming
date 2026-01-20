using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkedPlayerCombatComponent : NetworkBehaviour, INetworkCombat
{
    [SerializeField] private GameObject m_BombPrefab;
    [SerializeField] private float m_BombSpeed = 2.0f;

    void INetworkCombat.Initialise()
    {
        
    }

    void INetworkCombat.Handle_Action(InputAction.CallbackContext context)
    {
        DropBombRpc();
    }

    [Rpc(SendTo.Server)]
    private void DropBombRpc(RpcParams rpcParams = default)
    {
        GameObject obj = Instantiate(m_BombPrefab, transform.position + ((transform.forward * 2) + Vector3.up), Quaternion.identity);
        obj.GetComponent<NetworkObject>().Spawn();

        obj.GetComponent<Rigidbody>().AddForce(transform.forward * m_BombSpeed, ForceMode.Impulse);

    }
}
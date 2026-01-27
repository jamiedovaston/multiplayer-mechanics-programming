using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkedPlayerBatCombatComponent : NetworkBehaviour, INetworkCombat
{
    [SerializeField] private float m_HitSpeed = 2.0f;
    [SerializeField] private LayerMask m_BombLayer;
    [SerializeField] private GameObject m_DebugSphere;

    void INetworkCombat.Initialise()
    {

    }

    void INetworkCombat.Handle_Action(InputAction.CallbackContext context)
    {
        HitRpc();
    }

    [Rpc(SendTo.Server)]
    private void HitRpc(RpcParams rpcParams = default)
    {
        Collider[] colliders = Physics.OverlapSphere((transform.position + Vector3.up) + transform.forward * 3.0f, 3.0f, m_BombLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Rigidbody>().AddForce(transform.forward * m_HitSpeed, ForceMode.Impulse);
        }
    }
}
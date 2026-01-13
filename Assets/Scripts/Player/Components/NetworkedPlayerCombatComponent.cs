using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkedPlayerCombatComponent : NetworkBehaviour, INetworkCombat
{
    [SerializeField] private GameObject m_BombPrefab;

    void INetworkCombat.Initialise()
    {
        
    }

    void INetworkCombat.Handle_Action(InputAction.CallbackContext context)
    {
        GameObject obj = Instantiate(m_BombPrefab, transform.position + (Vector3.up * 2), Quaternion.identity);
        obj.GetComponent<NetworkObject>().Spawn();
    }
}
using System.Collections;
using Unity.Netcode;
using UnityEngine;

[SelectionBase]
public class NetworkedCoin : NetworkBehaviour
{
    [SerializeField] private GameObject m_CoinParticle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
            CoinCollectedRpc();
    }

    [Rpc(SendTo.Server)]
    private void CoinCollectedRpc(RpcParams rpcParams = default)
    {
        Debug.Log("Coin Collected!");
        CoinCollectedVFXRpc();

        NetworkObject.Despawn();
        Destroy(gameObject);
    }

    [Rpc(SendTo.Everyone)]
    private void CoinCollectedVFXRpc() => Destroy(Instantiate(m_CoinParticle, transform.position + Vector3.up, Quaternion.identity), 1.0f);
}
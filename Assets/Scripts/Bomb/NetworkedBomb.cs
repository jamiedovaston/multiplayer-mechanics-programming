using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkedBomb : NetworkBehaviour
{
    [SerializeField] private float m_BombExplodeTime = 3.0f;
    [SerializeField] private float m_BombExplodeRadius = 3.0f;
    [SerializeField] private float m_BombExplodeForce = 3.0f, m_BombExplodeUpwardsForce = 2.0f;
    [SerializeField] private LayerMask m_ExplosionLayers;
    [SerializeField] private GameObject m_ExplosionVFX;

    Coroutine m_BombRoutine;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        m_BombRoutine = StartCoroutine(C_BombCoroutine());
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        StopCoroutine(m_BombRoutine);
    }

    IEnumerator C_BombCoroutine()
    {
        yield return new WaitForSeconds(m_BombExplodeTime);

        ExplosionRpc();
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }

    [Rpc(SendTo.Everyone)]
    private void ExplosionRpc()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_BombExplodeRadius, m_ExplosionLayers);
        foreach (Collider hit in hitColliders)
        {
            if (hit.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                rb.AddExplosionForce(m_BombExplodeForce, transform.position, m_BombExplodeRadius, m_BombExplodeUpwardsForce, ForceMode.Impulse);
            }
        }
        Instantiate(m_ExplosionVFX, transform.position, Quaternion.identity);
    }
}

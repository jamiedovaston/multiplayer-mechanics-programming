using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkedPlayerBatCombatComponent : NetworkBehaviour, INetworkCombat
{
    [SerializeField] private float m_HitSpeed = 2.0f;
    [SerializeField] private LayerMask m_BombLayer;
    [SerializeField] private GameObject m_HitEffect;
    [SerializeField] private GameObject m_BatModel;

    private Animator m_Animator;

    private void Awake()
    {
        m_BatModel.SetActive(false);
    }

    void INetworkCombat.Initialise(Animator animator)
    {
        m_Animator = animator;
        m_Animator.SetInteger("Weapon", 0);
        m_BatModel.SetActive(true);
    }

    void INetworkCombat.Handle_Action(InputAction.CallbackContext context)
    {
        HitRpc();
    }

    [Rpc(SendTo.Server)]
    private void HitRpc(RpcParams rpcParams = default)
    {
        m_Animator.SetTrigger("Use");

        Collider[] colliders = Physics.OverlapSphere((transform.position + Vector3.up) + transform.forward * 1.0f, 2.0f, m_BombLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            Instantiate(m_HitEffect, colliders[i].transform.position, Quaternion.identity);
            colliders[i].GetComponent<Rigidbody>().AddForce(transform.forward * m_HitSpeed, ForceMode.Impulse);
        }
    }

    public void Enable(bool enable) => this.enabled = enable;
}
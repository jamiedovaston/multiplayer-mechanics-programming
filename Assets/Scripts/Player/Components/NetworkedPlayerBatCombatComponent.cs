using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkedPlayerBatCombatComponent : NetworkBehaviour, INetworkCombat
{
    [SerializeField] private float m_HitSpeed = 2.0f;
    [SerializeField] private int m_Damage;
    [SerializeField] private LayerMask m_HitLayer;
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
        m_Animator.SetTrigger("Use");
    }

    [Rpc(SendTo.Server)]
    private void HitRpc(RpcParams rpcParams = default)
    {
        Collider[] colliders = Physics.OverlapSphere((transform.position + Vector3.up) + transform.forward * 1.0f, 2.0f, m_HitLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            if(colliders[i].GetComponent<Rigidbody>()) colliders[i].GetComponent<Rigidbody>().AddForce(transform.forward * m_HitSpeed, ForceMode.Impulse);
            Instantiate(m_HitEffect, colliders[i].transform.position, Quaternion.identity);

            IDamageable dam = colliders[i].GetComponent<IDamageable>();
            if (dam != null) dam.DamageRpc(m_Damage, PlayerType.Bat);
        }
    }

    public void Enable(bool enable) => this.enabled = enable;
}
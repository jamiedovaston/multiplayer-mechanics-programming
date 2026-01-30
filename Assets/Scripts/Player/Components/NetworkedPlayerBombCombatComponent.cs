using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkedPlayerBombCombatComponent : NetworkBehaviour, INetworkCombat
{
    private Animator m_Animator;
    [SerializeField] private GameObject m_BombPrefab;
    [SerializeField] private float m_BombSpeed = 2.0f;
    [SerializeField] private GameObject m_BombModel;

    private void Awake()
    {
        m_BombModel.SetActive(false);
    }

    void INetworkCombat.Initialise(Animator animator)
    {
        m_Animator = animator;
        m_Animator.SetInteger("Weapon", 1);
        m_BombModel.SetActive(true);
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

        m_Animator.SetTrigger("Use");
    }

    public void Enable(bool enable) => this.enabled = enable;
}

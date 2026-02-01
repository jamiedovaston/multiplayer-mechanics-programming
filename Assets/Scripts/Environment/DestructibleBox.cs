using System;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using Unity.VisualScripting;
using UnityEngine;

public interface IDamageable
{
    public void DamageRpc(int damage, PlayerType type);
}

public class DestructibleBox : NetworkBehaviour, IDamageable
{
    public event Action OnDestroyed;

    [SerializeField] private PlayerType m_BoxType;
    [SerializeField] private int m_MaxHealth;
    [SerializeField] private MeshRenderer m_BoxMesh;
    private NetworkVariable<int> m_Health = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if(IsServer) m_Health.Value = m_MaxHealth;

        m_BoxMesh.material.color = m_BoxType == PlayerType.Bat ? Color.blue : Color.red;
    }

    [Rpc(SendTo.Server)]
    public void DamageRpc(int damage, PlayerType type)
    {
        if (type == m_BoxType)
        {
            m_Health.Value -= damage;

            if (m_Health.Value <= 0)
            {
                OnDestroyed?.Invoke();
                GetComponent<NetworkObject>().Despawn(true);
            }

        }
    }
}

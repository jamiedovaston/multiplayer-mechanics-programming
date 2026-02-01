using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bridge : NetworkBehaviour
{
    [SerializeField] private PlayerType m_BoxType;
    [SerializeField] private bool isBoth = false;
    [SerializeField] private MeshRenderer m_BoxMesh;

    private NetworkVariable<bool> m_IsBroken =
    new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone,
                                     NetworkVariableWritePermission.Server);

    public List<DestructibleBox> m_Boxes = new List<DestructibleBox>();
    private int m_NumBoxes;
    [SerializeField] private float m_TargetRotation;
    [SerializeField] private float m_RotationSpeed = 30f; // degrees per second
    
    private float m_CurrentRotation;

    public override void OnNetworkSpawn()
    {
        m_CurrentRotation = transform.localEulerAngles.x;

        m_BoxMesh.material.color = isBoth ? Color.white : (m_BoxType == PlayerType.Bat ? Color.blue : Color.red);

        if (!IsServer) return;
        m_NumBoxes = m_Boxes.Count;

        foreach (DestructibleBox box in m_Boxes)
        {
            box.OnDestroyed += BoxDestroyed;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        foreach (DestructibleBox box in m_Boxes)
        {
            box.OnDestroyed -= BoxDestroyed;
        }

    }

    private void BoxDestroyed()
    {
        m_NumBoxes--;

        if (m_NumBoxes <= 0 && !m_IsBroken.Value)
        {
            Debug.Log("Bridge Broken!");
            m_IsBroken.Value = true;
        }
    }

    private void Update()
    {
        if (!m_IsBroken.Value) return;

        m_CurrentRotation = Mathf.MoveTowards(
            m_CurrentRotation,
            m_TargetRotation,
            m_RotationSpeed * Time.deltaTime
        );

        transform.localRotation = Quaternion.Euler(
            m_CurrentRotation,
            0f,
            0f
        );
    }

}
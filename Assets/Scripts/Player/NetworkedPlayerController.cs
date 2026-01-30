using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(NetworkRigidbody))]
public class NetworkedPlayerController : NetworkBehaviour
{
    Rigidbody rb;
    Animator animator;
    InputSystem_Actions m_InputActions;

    // COMPONENTS
    INetworkMovement m_Movement;
    INetworkCombat m_BatCombat, m_BombCombat;
    INetworkRotation m_Rotation;

    [Rpc(SendTo.Everyone)]
    public void InitialiseRpc(bool IsBat)
    {
        if (IsBat)
        {
            m_BatCombat.Enable(true);
            m_BatCombat.Initialise(animator);
            m_InputActions.Player.Bat.performed += m_BatCombat.Handle_Action;
        }
        else
        {
            m_BombCombat.Enable(true);
            m_BombCombat.Initialise(animator);
            m_InputActions.Player.Bomb.performed += m_BombCombat.Handle_Action;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        m_InputActions = new InputSystem_Actions();

        m_Movement = GetComponent<INetworkMovement>();
        Debug.Assert(m_Movement != null, "Network Movement component is missing!", this);
        m_Movement.Initialise(rb, animator);

        m_BatCombat = GetComponent<NetworkedPlayerBatCombatComponent>();
        m_BombCombat = GetComponent<NetworkedPlayerBombCombatComponent>();
        Debug.Assert(m_BatCombat != null, "Network Bat Combat component is missing!", this);
        Debug.Assert(m_BombCombat != null , "Network Bomb Combat component is missing!", this);
        m_BatCombat.Enable(false);
        m_BombCombat.Enable(false);

        m_Rotation = GetComponent<INetworkRotation>();
        Debug.Assert(m_Rotation != null, "Network Combat component is missing!", this);
        m_Rotation.Initialise();
    }

    private void OnEnable()
    {
        CameraManager.instance.AssignTargetToTargetGroup(this.transform);
    }

    private void OnDisable()
    {
        CameraManager.instance.RemoveTargetFromTargetGroup(this.transform);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        m_InputActions.Enable();

        m_InputActions.Player.Move.performed += m_Movement.Handle_Action;
        m_InputActions.Player.Move.canceled += m_Movement.Handle_Action;

        m_InputActions.Player.Point.performed += m_Rotation.Handle_Action;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }

        m_InputActions.Disable();

        m_InputActions.Player.Move.performed -= m_Movement.Handle_Action;
        m_InputActions.Player.Move.canceled -= m_Movement.Handle_Action;

        m_InputActions.Player.Bat.performed -= m_BatCombat.Handle_Action;
        m_InputActions.Player.Bomb.performed -= m_BombCombat.Handle_Action;

        m_InputActions.Player.Point.performed -= m_Rotation.Handle_Action;
    }
}

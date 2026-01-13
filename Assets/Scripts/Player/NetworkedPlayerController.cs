using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(NetworkRigidbody))]
public class NetworkedPlayerController : NetworkBehaviour
{
    Rigidbody rb;
    InputSystem_Actions m_InputActions;

    // COMPONENTS
    INetworkMovement m_Movement;
    INetworkCombat m_Combat;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        m_InputActions = new InputSystem_Actions();

        m_Movement = GetComponent<INetworkMovement>();
        Debug.Assert(m_Movement != null, "Network Movement component is missing!", this);
        m_Movement.Initialise(rb);

        m_Combat = GetComponent<INetworkCombat>();
        Debug.Assert(m_Combat != null, "Network Combat component is missing!", this);
        m_Combat.Initialise();
    }

    private void OnEnable()
    {
        CameraManager.instance.AssignTargetToTargetGroup(this.transform);
    }

    private void Start()
    {
        if (!IsOwner) { return; }

        Debug.Log("Input Assigned!");
        m_InputActions.Enable();

        m_InputActions.Player.Move.performed += m_Movement.Handle_Action;
        m_InputActions.Player.Move.canceled += m_Movement.Handle_Action;

        m_InputActions.Player.Attack.performed += m_Combat.Handle_Action;
    }

    public override void OnDestroy()
    {
        if (!IsOwner) { return; }

        m_InputActions.Disable();

        m_InputActions.Player.Move.performed -= m_Movement.Handle_Action;
        m_InputActions.Player.Move.canceled -= m_Movement.Handle_Action;

        m_InputActions.Player.Attack.performed -= m_Combat.Handle_Action;

        base.OnDestroy();
    }

    private void OnDisable()
    {
        CameraManager.instance.RemoveTargetFromTargetGroup(this.transform);
    }
}

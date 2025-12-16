using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]
[RequireComponent(typeof(NetworkRigidbody))]
public class NetworkedPlayerController : NetworkBehaviour
{
    Rigidbody rb;
    InputSystem_Actions m_InputActions;

    Vector2 direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        m_InputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        m_InputActions.Enable();

        m_InputActions.Player.Jump.performed += Handle_JumpPerformed;

        m_InputActions.Player.Move.performed += Handle_MovePerformed;
        m_InputActions.Player.Move.canceled += Handle_MoveCanceled;
    }

    private void OnDisable()
    {
        m_InputActions.Disable();

        m_InputActions.Player.Jump.performed -= Handle_JumpPerformed;

        m_InputActions.Player.Move.performed -= Handle_MovePerformed;
        m_InputActions.Player.Move.canceled -= Handle_MoveCanceled;
    }

    private void Update()
    {
        rb.AddForce(direction);
    }

    private void Handle_MovePerformed(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        Vector3 inp = new Vector3(context.ReadValue<Vector2>().x, 0.0f, context.ReadValue<Vector2>().y);
        direction = transform.forward + transform.right;
        direction = inp * 5.0f;
    }

    private void Handle_MoveCanceled(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        Vector3 inp = new Vector3(context.ReadValue<Vector2>().x, 0.0f, context.ReadValue<Vector2>().y);
        direction = transform.forward + transform.right;
        direction = direction * inp * 5.0f;
    }

    private void Handle_JumpPerformed(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
    }
}

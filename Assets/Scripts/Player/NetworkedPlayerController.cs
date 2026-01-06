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

    Vector3 direction;
    [SerializeField] private float speed = 8.0f;

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

    private void FixedUpdate()
    {
        rb.AddForce(direction);
    }

    private void Handle_MovePerformed(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        Debug.Log($"MOVEMENT: X: {context.ReadValue<Vector2>().x} Y: {context.ReadValue<Vector2>().y}");

        Vector2 inp = context.ReadValue<Vector2>();
        direction = transform.forward * inp.y + transform.right * inp.x;
        direction *= speed;

        Debug.Log($"DIRECTION: { direction }");
    }

    private void Handle_MoveCanceled(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        direction = Vector2.zero;
    }

    private void Handle_JumpPerformed(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
    }
}

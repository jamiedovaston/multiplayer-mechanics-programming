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

    Vector3 input;
    [SerializeField] private float speed = 8.0f;
    [SerializeField] private float counterMovement = 0.175f;
    [SerializeField] private float jumpSpeed = 15.90f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        m_InputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        m_InputActions.Enable();

        m_InputActions.Player.Move.performed += Handle_MoveActioned;
        m_InputActions.Player.Move.canceled += Handle_MoveActioned;
    }

    private void OnDisable()
    {
        m_InputActions.Disable();

        m_InputActions.Player.Move.performed -= Handle_MoveActioned;
        m_InputActions.Player.Move.canceled -= Handle_MoveActioned;
    }

    private void FixedUpdate()
    {
        Movement();
    }

    public void Movement()
    {
        rb.AddForce(Vector3.down * Time.fixedDeltaTime * 450.0f);

        CounterMovement();

        rb.AddForce(transform.right * input.x * speed * Time.fixedDeltaTime);
        rb.AddForce(transform.forward * input.y * speed * Time.fixedDeltaTime);
    }

    public void CounterMovement()
    {
        rb.AddForce(transform.right * speed * Time.fixedDeltaTime * -rb.linearVelocity.x * counterMovement);
        rb.AddForce(transform.forward * speed * Time.fixedDeltaTime * -rb.linearVelocity.z * counterMovement);
    }

    private void Handle_MoveActioned(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        input = context.ReadValue<Vector2>();
    }
}

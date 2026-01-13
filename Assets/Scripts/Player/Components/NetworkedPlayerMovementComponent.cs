using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class NetworkedPlayerMovementComponent : NetworkBehaviour, INetworkMovement
{
    Rigidbody rb;
    Vector2 input;

    [SerializeField] private float speed = 8.0f;
    [SerializeField] private float counterMovement = 0.175f;

    void INetworkMovement.Initialise(Rigidbody rb)
    {
        Debug.Log("Initialised!");
        this.rb = rb;
    }

    void INetworkMovement.Handle_Action(InputAction.CallbackContext context)
    {
        Debug.Log("Movement Input!");
        input = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        Movement();
    }

    private void Movement()
    {
        rb.AddForce(Vector3.down * Time.fixedDeltaTime * 450.0f);

        CounterMovement();

        rb.AddForce(transform.right * input.x * speed * Time.fixedDeltaTime);
        rb.AddForce(transform.forward * input.y * speed * Time.fixedDeltaTime);
    }

    private void CounterMovement()
    {
        rb.AddForce(transform.right * speed * Time.fixedDeltaTime * -rb.linearVelocity.x * counterMovement);
        rb.AddForce(transform.forward * speed * Time.fixedDeltaTime * -rb.linearVelocity.z * counterMovement);
    }
}

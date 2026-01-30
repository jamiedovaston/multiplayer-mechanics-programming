using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class NetworkedPlayerMovementComponent : NetworkBehaviour, INetworkMovement
{
    Rigidbody rb;
    Vector2 input;
    Animator animator;

    [SerializeField] private float speed = 8.0f;
    [SerializeField] private float counterMovement = 0.175f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    private bool isGrounded;

    void INetworkMovement.Initialise(Rigidbody rb, Animator animator)
    {
        this.rb = rb;
        this.animator = animator;
    }

    void INetworkMovement.Handle_Action(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        CheckGrounded();
        Movement();
        UpdateAnimator();
    }

    private void Movement()
    {
        CounterMovement();

        rb.AddForce(Vector3.right * input.x * speed * Time.fixedDeltaTime);
        rb.AddForce(Vector3.forward * input.y * speed * Time.fixedDeltaTime);
    }

    private void CounterMovement()
    {
        rb.AddForce(Vector3.right * speed * Time.fixedDeltaTime * -rb.linearVelocity.x * counterMovement);
        rb.AddForce(Vector3.forward * speed * Time.fixedDeltaTime * -rb.linearVelocity.z * counterMovement);
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.OverlapSphere(
            transform.position,
            groundCheckRadius,
            groundLayer,
            QueryTriggerInteraction.Ignore
        ).Length > 0;
    }

    private void UpdateAnimator()
    {
        animator.SetBool("IsGrounded", isGrounded);

        Vector3 worldMoveDir = new Vector3(input.x, 0f, input.y);

        if (worldMoveDir.sqrMagnitude < 0.001f)
        {
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveY", 0f);
            return;
        }

        Vector3 localMoveDir = transform.InverseTransformDirection(worldMoveDir.normalized);

        animator.SetFloat("MoveX", localMoveDir.x);
        animator.SetFloat("MoveY", localMoveDir.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            groundCheckRadius
        );
    }
}

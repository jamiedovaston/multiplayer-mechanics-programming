using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkedPlayerRotationComponent : NetworkBehaviour, INetworkRotation
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private GameObject rotationObject;

    void INetworkRotation.Initialise()
    {

    }

    void INetworkRotation.Handle_Action(InputAction.CallbackContext context)
    {
        Vector2 mousePos = context.ReadValue<Vector2>();

        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 999f, groundMask))
        {
            Vector3 lookPos = hit.point;
            lookPos.y = transform.position.y;

            transform.LookAt(lookPos);
        }
    }

    public override void OnNetworkSpawn() =>
        rotationObject.SetActive(IsLocalPlayer);
}
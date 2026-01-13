using UnityEngine;
using UnityEngine.InputSystem;

public interface INetworkMovement
{
    public void Initialise(Rigidbody rb);
    public void Handle_Action(InputAction.CallbackContext context);
}

public interface INetworkCombat
{
    public void Initialise();
    public void Handle_Action(InputAction.CallbackContext context);
}

public interface INetworkRotation
{
    public void Initialise();
    public void Handle_Action(InputAction.CallbackContext context);
}
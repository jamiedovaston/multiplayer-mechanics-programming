using UnityEngine;
using UnityEngine.InputSystem;

public interface INetworkMovement
{
    public void Initialise(Rigidbody rb, Animator animator);
    public void Handle_Action(InputAction.CallbackContext context);
}

public interface INetworkCombat
{
    public void Initialise(Animator animator);
    public void Handle_Action(InputAction.CallbackContext context);
    public void Enable(bool enable);
}

public interface INetworkRotation
{
    public void Initialise();
    public void Handle_Action(InputAction.CallbackContext context);
}
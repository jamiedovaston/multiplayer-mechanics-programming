using Unity.Netcode.Components;
using UnityEngine;

public class CustomNetworkAnimator : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}

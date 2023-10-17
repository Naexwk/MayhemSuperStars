using Unity.Netcode.Components;
using UnityEngine;

// Modificación al NetworkAnimator para que reciba datos de cliente
namespace Unity.Multiplayer.Samples.Utilities.ClientAuthority
{
    [DisallowMultipleComponent]
    public class ClientNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}

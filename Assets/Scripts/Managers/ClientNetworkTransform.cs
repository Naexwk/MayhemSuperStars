using Unity.Netcode.Components;
using UnityEngine;

// Modificación al NetworkTransform para que reciba datos de cliente
namespace Unity.Multiplayer.Samples.Utilities.ClientAuthority
{
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class ClientNetworkTransform : NetworkTransform
{
    public override void OnNetworkSpawn()
    {
        // Sadece sahibi hareket ettirebili
        base.OnNetworkSpawn();
        CanCommitToTransform = IsOwner;
    }

    protected override bool OnIsServerAuthoritative()
    {
        return false; // Client authoritative
        // Tells networking system that server is NOT in charge of this object's transform
    }
}

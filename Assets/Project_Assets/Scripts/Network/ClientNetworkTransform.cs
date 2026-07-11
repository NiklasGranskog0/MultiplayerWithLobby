using Unity.Netcode.Components;

namespace Project_Assets.Scripts.Network
{
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative() => false;
    }
}

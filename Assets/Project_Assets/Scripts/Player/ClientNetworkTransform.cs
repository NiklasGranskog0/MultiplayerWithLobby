using Unity.Netcode.Components;

namespace Project_Assets.Scripts.Player
{
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative() => false;
    }
}

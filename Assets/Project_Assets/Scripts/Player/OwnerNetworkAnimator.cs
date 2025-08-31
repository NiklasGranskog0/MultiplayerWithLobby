using Unity.Netcode.Components;

namespace Project_Assets.Scripts.Player
{
    public class OwnerNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative() => false;
    }
}

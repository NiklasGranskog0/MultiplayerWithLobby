using Unity.Netcode.Components;

namespace Project_Assets.Scripts.Network
{
    public class OwnerNetworkAnimator : NetworkAnimator
    {
        // TODO: Probably move AnimationManager & PlayerAnimations to this script
        protected override bool OnIsServerAuthoritative() => false;
    }
}

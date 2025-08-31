using Project_Assets.Scripts.Animations;
using Project_Assets.Scripts.Enums;
using UnityEngine;

namespace Project_Assets.Scripts.Player
{
    public class PlayerAnimations : MonoBehaviour
    {
        [SerializeField] private AnimationManager<PlayerAnim> playerAnimManager;
        
        public void Initialize()
        {
            playerAnimManager.Initialize(PlayerAnim.IdleBattleSwordAndShield);
        }

        public void OnUpdate(float speed, Vector3 velocity)
        {
            UpdatePlayerAnimation(speed, velocity);
        }
        
        private void UpdatePlayerAnimation(float speed, Vector3 velocity)
        {
            if (speed < 0.1f)
            {
                playerAnimManager.Play(PlayerAnim.IdleBattleSwordAndShield, 0);
                return;
            }

            if (velocity.x != 0f && velocity.z != 0f)
            {
                switch (velocity)
                {
                    case { x: > 0f, z: > 0f }: // Right up
                        playerAnimManager.Play(PlayerAnim.MoveRGTBattleRMSwordAndShield, 0);
                        break;
                    case { x: < 0f, z: < 0f }: // Back left
                        playerAnimManager.Play(PlayerAnim.MoveLFTBattleRMSwordAndShield, 0);
                        break;
                    case { x: > 0f, z: < 0f }: // Right down
                        playerAnimManager.Play(PlayerAnim.MoveRGTBattleRMSwordAndShield, 0);
                        break;
                    case { x: < 0f, z: > 0f }: // Left up
                        playerAnimManager.Play(PlayerAnim.MoveLFTBattleRMSwordAndShield, 0);
                        break;
                }
            }
            else
            {
                switch (velocity.z)
                {
                    case > 0f: // Forwards 
                        playerAnimManager.Play(PlayerAnim.MoveFWDNormalRMSwordAndShield, 0);
                        break;
                    case < 0f: // Backwards
                        playerAnimManager.Play(PlayerAnim.MoveBWDBattleRMSwordAndShield, 0);
                        break;
                }

                switch (velocity.x)
                {
                    case > 0f: // Right
                        playerAnimManager.Play(PlayerAnim.MoveRGTBattleRMSwordAndShield, 0);
                        break;
                    case < 0f: // Left
                        playerAnimManager.Play(PlayerAnim.MoveLFTBattleRMSwordAndShield, 0);
                        break;
                }
            }
        }
    }
}
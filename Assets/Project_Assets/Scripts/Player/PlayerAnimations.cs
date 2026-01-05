using Project_Assets.Scripts.Animations;
using Project_Assets.Scripts.Enums;
using UnityEngine;

namespace Project_Assets.Scripts.Player
{
    public class PlayerAnimations : MonoBehaviour
    {
        [SerializeField] private AnimationManager<PlayerAnim> m_playerAnimManager;
        
        public void Initialize()
        {
            m_playerAnimManager.Initialize(PlayerAnim.IdleBattleSwordAndShield);
        }

        public void OnUpdate(float speed, Vector3 velocity)
        {
            UpdatePlayerAnimation(speed, velocity);
        }
        
        private void UpdatePlayerAnimation(float speed, Vector3 velocity)
        {
            if (speed < 0.1f)
            {
                m_playerAnimManager.Play(PlayerAnim.IdleBattleSwordAndShield, 0);
                return;
            }

            if (velocity.x != 0f && velocity.z != 0f)
            {
                switch (velocity)
                {
                    case { x: > 0f, z: > 0f }: // Right up
                        m_playerAnimManager.Play(PlayerAnim.MoveRgtBattleRmSwordAndShield, 0);
                        break;
                    case { x: < 0f, z: < 0f }: // Back left
                        m_playerAnimManager.Play(PlayerAnim.MoveLftBattleRmSwordAndShield, 0);
                        break;
                    case { x: > 0f, z: < 0f }: // Right down
                        m_playerAnimManager.Play(PlayerAnim.MoveRgtBattleRmSwordAndShield, 0);
                        break;
                    case { x: < 0f, z: > 0f }: // Left up
                        m_playerAnimManager.Play(PlayerAnim.MoveLftBattleRmSwordAndShield, 0);
                        break;
                }
            }
            else
            {
                switch (velocity.z)
                {
                    case > 0f: // Forwards 
                        m_playerAnimManager.Play(PlayerAnim.MoveFwdNormalRmSwordAndShield, 0);
                        break;
                    case < 0f: // Backwards
                        m_playerAnimManager.Play(PlayerAnim.MoveBwdBattleRmSwordAndShield, 0);
                        break;
                }

                switch (velocity.x)
                {
                    case > 0f: // Right
                        m_playerAnimManager.Play(PlayerAnim.MoveRgtBattleRmSwordAndShield, 0);
                        break;
                    case < 0f: // Left
                        m_playerAnimManager.Play(PlayerAnim.MoveLftBattleRmSwordAndShield, 0);
                        break;
                }
            }
        }
    }
}
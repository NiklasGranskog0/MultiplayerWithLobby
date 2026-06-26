using Project_Assets.Scripts.Animations;
using Project_Assets.Scripts.Enums;
using UnityEngine;

namespace Project_Assets.Scripts.Player
{
    public class PlayerAnimations : MonoBehaviour
    {
        [SerializeField] private AnimationManager<PlayerAnim> m_playerAnimManager;
        private PlayerMovement m_playerMovement;
        
        public void Initialize(PlayerMovement playerMovement)
        {
            m_playerAnimManager.Initialize(PlayerAnim.IdleBattleSwordAndShield);
            m_playerMovement = playerMovement;
        }

        // TODO: Get velocity / Speed from NavmeshAgent
        public void OnUpdate()
        {
            if (m_playerMovement == null) return;
            
            var speed = m_playerMovement.PlayerAgent.velocity.magnitude;
            UpdatePlayerAnimation(speed);
        }
        
        private void UpdatePlayerAnimation(float speed)
        {
            switch (speed)
            {
                case < 0.1f:
                    m_playerAnimManager.Play(PlayerAnim.IdleBattleSwordAndShield, 0);
                    return;
                case > 0.1f:
                    m_playerAnimManager.Play(PlayerAnim.MoveFwdNormalRmSwordAndShield, 0);
                    break;
            }
        }
    }
}
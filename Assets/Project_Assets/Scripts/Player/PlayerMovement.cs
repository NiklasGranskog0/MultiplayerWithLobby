using Project_Assets.Scripts.ScriptableObjects;
using UnityEngine;

namespace Project_Assets.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float m_movementSpeed = 5f;
        private PlayerInputs m_playerInputs;

        private Vector3 m_playerMoveDirection;
        private Vector3 m_lastPosition;
        private Vector3 m_velocity;

        public float Speed => new Vector3(m_velocity.x, 0f, m_velocity.z).magnitude;
        public Vector3 Velocity => m_velocity;

        public void Initialize(PlayerInputs playerInputs) => m_playerInputs = playerInputs;
        
        private void Awake()
        {
            m_playerInputs.OnMovementEvent += SetPlayerMoveDirection;
            m_lastPosition = transform.position;
        }

        public void OnUpdate()
        {
            m_lastPosition = transform.position;

            MoveTransform();
            CalculateVelocity();
        }

        private void CalculateVelocity()
        {
            m_velocity = (transform.position - m_lastPosition) / Time.deltaTime;
        }

        private void MoveTransform()
        {
            if (m_playerMoveDirection.sqrMagnitude > 0f)
            {
                transform.position += m_playerMoveDirection.normalized * (m_movementSpeed * Time.deltaTime);
            }
        }
        
        private void SetPlayerMoveDirection(Vector2 direction)
        {
            m_playerMoveDirection.x = direction.x;
            m_playerMoveDirection.z = direction.y;
        }
    }
}

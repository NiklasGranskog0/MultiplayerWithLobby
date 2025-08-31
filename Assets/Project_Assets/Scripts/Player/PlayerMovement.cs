using Project_Assets.Scripts.ScriptableObjects;
using UnityEngine;

namespace Project_Assets.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 5f;
        private PlayerInputs m_PlayerInputs;

        private Vector3 m_PlayerMoveDirection;
        private Vector3 m_LastPosition;
        private Vector3 m_Velocity;

        public float Speed => new Vector3(m_Velocity.x, 0f, m_Velocity.z).magnitude;
        public Vector3 Velocity => m_Velocity;

        public void Initialize(PlayerInputs playerInputs) => m_PlayerInputs = playerInputs;
        
        private void Awake()
        {
            m_PlayerInputs.OnMovementEvent += SetPlayerMoveDirection;
            m_LastPosition = transform.position;
        }

        public void OnUpdate()
        {
            m_LastPosition = transform.position;

            MoveTransform();
            CalculateVelocity();
        }

        private void CalculateVelocity()
        {
            m_Velocity = (transform.position - m_LastPosition) / Time.deltaTime;
        }

        private void MoveTransform()
        {
            if (m_PlayerMoveDirection.sqrMagnitude > 0f)
            {
                transform.position += m_PlayerMoveDirection.normalized * (movementSpeed * Time.deltaTime);
            }
        }
        
        private void SetPlayerMoveDirection(Vector2 direction)
        {
            m_PlayerMoveDirection.x = direction.x;
            m_PlayerMoveDirection.z = direction.y;
        }
    }
}

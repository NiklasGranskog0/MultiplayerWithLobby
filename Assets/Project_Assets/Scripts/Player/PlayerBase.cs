using Project_Assets.Scripts.ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.Player
{
    public class PlayerBase : NetworkBehaviour
    {
        [SerializeField] private PlayerInputs projectInputs;

        private Vector3 m_PlayerDirection;
        private Vector3 m_MousePosition;

        private void Awake()
        {
            projectInputs.OnMovementEvent += PlayerDirection;
            projectInputs.OnCameraEvent += PlayerLookDirection;
        }

        private void PlayerLookDirection(Vector2 inputMousePosition)
        {
            m_MousePosition.x = inputMousePosition.x;
            m_MousePosition.y = inputMousePosition.y;
        }

        private void Update()
        {
            if (!IsOwner) return;

            Look();
            Move();
        }

        private void Look()
        {
            // Do not rotate while strafing (left/right), including diagonal movement
            if (Mathf.Abs(m_PlayerDirection.x) > 0.001f)
            {
                return;
            }

            var cam = Camera.main;
            if (cam == null) return;
            
            var ray = cam.ScreenPointToRay(m_MousePosition);
            var plane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));

            if (plane.Raycast(ray, out var enter))
            {
                var hitPoint = ray.GetPoint(enter);
                var lookDir = hitPoint - transform.position;
                lookDir.y = 0f;

                if (lookDir.sqrMagnitude > 0.0001f)
                {
                    transform.forward = lookDir.normalized;
                }
            }
        }

        private void Move()
        {
            // Strafe left/right relative to current facing without changing rotation
            var moveDir = (transform.right * m_PlayerDirection.x) + (transform.forward * m_PlayerDirection.z);
            
            if (moveDir.sqrMagnitude > 0.0001f)
            {
                transform.position += moveDir.normalized * (5f * Time.deltaTime);
            }
        }
        
        private void PlayerDirection(Vector2 vector2)
        {
            m_PlayerDirection.x = vector2.x;
            m_PlayerDirection.z = vector2.y;
        }
    }
}

using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.ScriptableObjects;
using UnityEngine;

namespace Project_Assets.Scripts.Player
{
    public class PlayerMouseTarget : MonoBehaviour
    {
        [SerializeField] private Camera m_playerCamera;
        [SerializeField] private PlayerInputs m_playerInputs;
        private Vector3 m_mousePosition;
        private ulong m_playerId; // TODO: Temp
        
        public Ray MouseRay => m_playerCamera.ScreenPointToRay(m_mousePosition);

        private void Awake()
        {
            ServiceLocator.Global.Register(this, ServiceLevel.Global, gameObject.scene.name);
            m_playerInputs.OnMouseMovedEvent += UpdateMouseLocation;
        }

        private void UpdateMouseLocation(Vector2 mousePosition) => m_mousePosition = mousePosition;
    }
}

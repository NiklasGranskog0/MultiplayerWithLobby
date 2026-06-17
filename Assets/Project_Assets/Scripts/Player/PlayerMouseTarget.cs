using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project_Assets.Scripts.Player
{
    public class PlayerMouseTarget : MonoBehaviour
    {
        [SerializeField] private Camera m_playerCamera;
        [SerializeField] private PlayerInputs m_playerInputs;
        private Vector3 m_mousePosition;
        private ulong m_playerId; // TODO: Might be useful or not
        
        public Ray MouseRay 
        {
            get
            {
                if (Mouse.current != null)
                {
                    m_mousePosition = Mouse.current.position.ReadValue();
                }
                
                return m_playerCamera.ScreenPointToRay(m_mousePosition);
            }
        } 

        private void Awake() => ServiceLocator.Global.Register(this, ServiceLevel.Global, gameObject.scene.name);
    }
}
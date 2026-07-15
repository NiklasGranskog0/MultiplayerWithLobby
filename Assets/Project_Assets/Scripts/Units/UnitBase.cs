using UnityEngine;

namespace Project_Assets.Scripts.Units
{
    public class UnitBase : MonoBehaviour
    {
        [SerializeField] private UnitMovement m_unitMovement;
        public string TeamTag;
        
        private void Start()
        {
            m_unitMovement.Initialize();
        }
    }
}

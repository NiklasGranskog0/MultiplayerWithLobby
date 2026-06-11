using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Game
{
    public class UIObjectSelector : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_selectedObjectName;
        [SerializeField] private Image m_selectedObjectImage;
        [SerializeField] private Camera m_playerCamera;
        [SerializeField] private UIInputs m_uiInputs;
        
        private GameObject m_selectedObject;

        private void Start()
        {
            m_uiInputs.OnRightClickEvent += OnClick;
        }

        private void OnClick()
        {
            
        }

        private void Awake() => ServiceLocator.Global.Register(this, ServiceLevel.Global, gameObject.scene.name);
    }
}

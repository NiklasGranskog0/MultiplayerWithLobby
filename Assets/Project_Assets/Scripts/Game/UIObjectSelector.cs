using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Player;
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
        [SerializeField] private LayerMask m_uiObjectSelectionLayer;
        [SerializeField] private PlayerInputs m_playerInputs;
        // [SerializeField] private PlayerCamera m_playerCamera;

        private GameObject m_selectedObject;
        private bool m_selectRequested;
        private const float k_raycastDistance = 500f;

        // TODO: At the start of the game have the player character as the selected object
        // private void Start() => m_playerInputs.OnLeftMouseClickEvent += OnClick;
        // private void OnClick() => m_selectRequested = true;

        // Raycast in late update to make sure we are casting from the correct camera position
        // TODO: m_selectedObjectImage.sprite = m_selectedObject.GetComponent<SpriteRenderer>().sprite;
        // private void LateUpdate()
        // {
        //     if (m_selectRequested)
        //     {
        //         var mouseRay = m_playerCamera.MouseRay;
        //
        //         if (Physics.Raycast(mouseRay, out var hitInfo, k_raycastDistance, m_uiObjectSelectionLayer))
        //         {
        //             if (m_selectRequested)
        //             {
        //                 Debug.Log($"Selected {hitInfo.collider.tag}");
        //                 m_selectedObject = hitInfo.collider.gameObject;
        //                 m_selectedObjectName.text = m_selectedObject.name;
        //             }
        //         }
        //     }
        //
        //     m_selectRequested = false;
        // }
        
        // private void Awake() => ServiceLocator.Global.Register(this, ServiceLevel.Global, gameObject.scene.name);
    }
}
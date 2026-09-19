using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.Interfaces;
using Project_Assets.Scripts.Player;
using Project_Assets.Scripts.ScriptableObjects;
using Project_Assets.Scripts.UtilityExtensions.Strings;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Game.UI
{
    public class ObjectTargeter : MonoBehaviour
    {
        [SerializeField] private LayerMask m_uiObjectSelectionLayer;

        private TextMeshProUGUI m_selectedObjectName;
        private RawImage m_selectedObjectRawImage;

        private PlayerInputs m_playerInputs;
        private PlayerCamera m_playerCamera;
        private UIObjects m_uiObjects;
        private ImageManager m_imageManager;

        private GameObject m_selectedObject;
        private bool m_selectionRequest;
        private const float k_raycastDistance = 500f;

        private string m_teamTag;

        public void Initialize(PlayerInputs playerInputs, PlayerCamera playerCamera, ISelectionObject defaultSelectedObject,
            Team teamTag)
        {
            m_playerInputs = playerInputs;
            m_playerCamera = playerCamera;
            m_teamTag = teamTag.ToString();
            
            m_playerInputs.OnLeftMouseClickEvent += OnClick;

            ServiceLocator.Global.Get(out m_uiObjects);
            ServiceLocator.Global.Get(out m_imageManager);

            m_selectedObjectName = m_uiObjects.SelectedObjectName;
            m_selectedObjectRawImage = m_uiObjects.SelectedObjectRawImage;
            m_selectedObjectRawImage.texture = m_imageManager.ImageCamera.targetTexture;

            // At the start of the game have the player character as the selected object
            m_selectedObjectName.text = defaultSelectedObject.Name;
            m_imageManager.LoadImage(defaultSelectedObject.ImageToLoad);
            defaultSelectedObject.SetGameMenuButtons();
            
            gameObject.SetActive(true);
        }

        private void Awake()
        {
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("Game"));
            gameObject.SetActive(false);
        } 
        
        private void OnClick() => m_selectionRequest = true;

        // Raycast in late update to make sure we are creating a raycast direction after the camera has moved
        private void LateUpdate()
        {
            if (m_selectionRequest)
            {
                var mouseRay = m_playerCamera.MouseRay;

                if (Physics.Raycast(mouseRay, out var hitInfo, k_raycastDistance, m_uiObjectSelectionLayer))
                {
                    if (!hitInfo.collider.gameObject.CompareTag(m_teamTag)) return;
                    
                    m_selectedObject = hitInfo.collider.gameObject;
                    
                    var selectionObject = m_selectedObject.GetComponent<ISelectionObject>();

                    selectionObject.SetGameMenuButtons();
                    m_imageManager.LoadImage(selectionObject.ImageToLoad);
                    m_selectedObjectName.text = selectionObject.Name;
                }
            }

            m_selectionRequest = false;
        }
    }
}
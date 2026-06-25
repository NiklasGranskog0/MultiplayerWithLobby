using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Game.UI;
using Project_Assets.Scripts.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Player
{
    public class ObjectTargeter : MonoBehaviour
    {
        [SerializeField] private LayerMask m_uiObjectSelectionLayer;

        // TODO: Name is just a placeholder for now, might replace it with something else
        // TODO: Might add health bar to the object
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

        public void Initialize(PlayerInputs playerInputs, PlayerCamera playerCamera,
            GameObject defaultSelectedObject, string teamTag)
        {
            m_playerInputs = playerInputs;
            m_playerCamera = playerCamera;
            m_teamTag = teamTag;

            ServiceLocator.Global.Get(out m_uiObjects);
            ServiceLocator.Global.Get(out m_imageManager);

            m_selectedObjectName = m_uiObjects.SelectedObjectName;
            m_selectedObjectRawImage = m_uiObjects.SelectedObjectRawImage;
            m_selectedObjectRawImage.texture = m_imageManager.ImageCamera.targetTexture;

            // At the start of the game have the player character as the selected object
            m_selectedObject = defaultSelectedObject;
            m_selectedObjectName.text = m_selectedObject.name;
            m_imageManager.LoadImage(ImageToLoad.Player);
        }

        private void Start() => m_playerInputs.OnLeftMouseClickEvent += OnClick;
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

                    // TODO: Get the selection objects menu buttons
                    // TODO: Load the image of the selected object m_ImageManager.LoadImage();
                    // TODO: m_imageManager.LoadImage(SelectedObject.GetComponent<ImageToLoad>); GO has a ImageToLoad enum to get or something

                    m_selectedObject = hitInfo.collider.gameObject;
                    m_selectedObjectName.text = m_selectedObject.name;
                }
            }

            m_selectionRequest = false;
        }
    }
}
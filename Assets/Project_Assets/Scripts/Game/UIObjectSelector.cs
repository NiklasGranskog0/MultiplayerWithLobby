using System;
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
        
        private GameObject m_selectedObject;
        private PlayerMouseTarget m_playerMouseTarget;

        // TODO: At the start of the game have the player character as the selected object
        private void Start() => m_playerInputs.OnLeftMouseClickEvent += OnClick;
        
        // TODO: m_selectedObjectImage.sprite = m_selectedObject.GetComponent<SpriteRenderer>().sprite;
        private void OnClick()
        {
            // PlayerMouseTarget is registered in the Game scene, which loads after the UI scene
            if (m_playerMouseTarget == null)
            {
                ServiceLocator.Global.Get(out m_playerMouseTarget);
            }
            
            // TODO: Does not always work, raycast sometimes disappears
            if (Physics.Raycast(m_playerMouseTarget.MouseRay, out var hitInfo, float.MaxValue,
                    m_uiObjectSelectionLayer))
            {
                m_selectedObject = hitInfo.collider.gameObject;
                m_selectedObjectName.text = m_selectedObject.name;
            }
        }

        private void Awake() => ServiceLocator.Global.Register(this, ServiceLevel.Global, gameObject.scene.name);
    }
}

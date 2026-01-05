using System;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using UnityEngine;

namespace Project_Assets.Scripts.Lobby
{
    public class PanelSwitcher : MonoBehaviour
    {
        [Header("Panels")] 
        [SerializeField] private GameObject m_gamesListPanel;
        [SerializeField] private GameObject m_createLobbyPanel;
        [SerializeField] private GameObject m_lobbyPanel;

        private void Awake()
        {
            ServiceLocator.ForSceneOf(this).Register(this, ServiceLevel.Scene, gameObject.scene.name);
        }
        
        public void SwitchPanel(LobbyPanel panel)
        {
            m_gamesListPanel.SetActive(false);
            m_createLobbyPanel.SetActive(false);
            m_lobbyPanel.SetActive(false);

            // m_gamePasswordInputField.text = string.Empty;
            // GameCodeInputField.text = string.Empty;

            switch (panel)
            {
                case LobbyPanel.GamePanel:
                    m_gamesListPanel.SetActive(true);
                    break;
                case LobbyPanel.CreatePanel:
                    m_createLobbyPanel.SetActive(true);
                    break;
                case LobbyPanel.Lobby:
                    m_lobbyPanel.SetActive(true);
                    break;
                case LobbyPanel.Loading:
                    break;
                case LobbyPanel.Game:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(panel), panel, null);
            }
        }
    }
}

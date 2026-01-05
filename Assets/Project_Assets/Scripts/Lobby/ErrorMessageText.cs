using System;
using System.Collections.Generic;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Structs;
using UnityEngine;

namespace Project_Assets.Scripts.Lobby
{
    public class ErrorMessageText : MonoBehaviour
    {
        [SerializeField] private List<ErrorPanels> m_errorPanels;
        
        private void Awake()
        {
            ServiceLocator.ForSceneOf(this).Register(this, ServiceLevel.Scene, gameObject.scene.name);
        }
        
        public void ShowError(string errorMessage, LobbyPanel panel)
        {
            switch (panel)
            {
                case LobbyPanel.GamePanel:
                    StartCoroutine(m_errorPanels[0].SetText(errorMessage).FadeOut(m_errorPanels[0].FadeDuration));
                    break;
                case LobbyPanel.CreatePanel:
                    StartCoroutine(m_errorPanels[1].SetText(errorMessage).FadeOut(m_errorPanels[1].FadeDuration));
                    break;
                case LobbyPanel.Lobby:
                    StartCoroutine(m_errorPanels[2].SetText(errorMessage).FadeOut(m_errorPanels[2].FadeDuration));
                    break;
                case LobbyPanel.Loading:
                case LobbyPanel.Game:
                default:
                    throw new ArgumentOutOfRangeException(nameof(panel), panel, null);
            }
        }
    }
}

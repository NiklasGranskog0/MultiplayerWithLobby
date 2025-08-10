using System;
using System.Collections.Generic;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework_TempName;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Structs;
using UnityEngine;

namespace Project_Assets.Scripts.Lobby
{
    public class ErrorMessageText : MonoBehaviour
    {
        [SerializeField] private List<ErrorPanels> errorPanels;
        
        private void Awake()
        {
            ServiceLocator.Global.Register(this, ServiceLevel.Global);
        }
        
        public void ShowError(string errorMessage, LobbyPanel panel)
        {
            switch (panel)
            {
                case LobbyPanel.Games:
                    StartCoroutine(errorPanels[0].SetText(errorMessage).FadeOut(errorPanels[0].fadeDuration));
                    break;
                case LobbyPanel.Create:
                    StartCoroutine(errorPanels[1].SetText(errorMessage).FadeOut(errorPanels[1].fadeDuration));
                    break;
                case LobbyPanel.Lobby:
                    StartCoroutine(errorPanels[2].SetText(errorMessage).FadeOut(errorPanels[2].fadeDuration));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(panel), panel, null);
            }
        }
    }
}

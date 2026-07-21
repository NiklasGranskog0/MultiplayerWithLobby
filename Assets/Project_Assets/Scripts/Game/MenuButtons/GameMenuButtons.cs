using System;
using System.Collections.Generic;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework;
using Project_Assets.Scripts.Framework.ExtensionScripts;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using UnityEngine;
using UnityEngine.Events;

namespace Project_Assets.Scripts.Game.MenuButtons
{
    public class GameMenuButtons : MonoBehaviour
    {
        public MenuButton[] MenuButtons = new MenuButton[12];

        private Dictionary<GameMenuButton, MenuButton> m_menuButtonDictionary;

        private void Awake()
        {
            Initialize();
            
            // TODO: Does not need to be global, can be scene-specific
            ServiceLocator.Global.Register(this, ServiceLevel.Global, gameObject.scene.name);
        }

        private void Initialize()
        {
            m_menuButtonDictionary = new Dictionary<GameMenuButton, MenuButton>()
            {
                { GameMenuButton.TopLeft, MenuButtons[0] },
                { GameMenuButton.TopMiddleLeft , MenuButtons[1] },
                { GameMenuButton.TopMiddleRight, MenuButtons[2] },
                { GameMenuButton.TopRight, MenuButtons[3] },
                
                { GameMenuButton.MiddleLeft, MenuButtons[4] },
                { GameMenuButton.MiddleCenterLeft, MenuButtons[5] },
                { GameMenuButton.MiddleCenterRight, MenuButtons[6] },
                { GameMenuButton.MiddleRight, MenuButtons[7] },
                
                { GameMenuButton.BottomLeft, MenuButtons[8] },
                { GameMenuButton.BottomMiddleLeft, MenuButtons[9] },
                { GameMenuButton.BottomMiddleRight, MenuButtons[10] },
                { GameMenuButton.BottomRight, MenuButtons[11] },
            };
            
            ResetButtonBinds();
        }

        // TODO: Bind shortcut keys to player inputs
        public void BindButton(GameMenuButton buttonIndex, UnityEvent action, Sprite buttonImage, 
            string buttonToolTip = "", KeyCode shortcutKey = KeyCode.None, SerializedCallback<UnitType> callback = null)
        {
            var menuButton = m_menuButtonDictionary[buttonIndex];
            menuButton.Callback = callback;
            menuButton.ButtonComponent.onClick.AddListener(action.Invoke);
            menuButton.ImageComponent.sprite = buttonImage;
            menuButton.TextToolTip = buttonToolTip;
            menuButton.HasToolTip = !string.IsNullOrEmpty(buttonToolTip);
            menuButton.ShortcutKey = shortcutKey;
            menuButton.ButtonObject.SetActive(true);
        }
        
        public void ResetButtonBinds()
        {
            foreach (var menuButton in MenuButtons)
            {
                menuButton.ButtonComponent.onClick.RemoveAllListeners();
                menuButton.TextArea.text = "";
                menuButton.HasToolTip = false;
                menuButton.TextToolTip = "";
                menuButton.ShortcutKey = KeyCode.None;
                menuButton.gameObject.SetActive(false);
            }
        }
    }
}
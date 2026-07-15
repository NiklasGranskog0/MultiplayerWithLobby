using System.Collections.Generic;
using Project_Assets.Scripts.Enums;
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
        public void BindButton(GameMenuButton buttonIndex, UnityAction action, Sprite buttonImage, 
            string buttonToolTip = "", KeyCode shortcutKey = KeyCode.None)
        {
            m_menuButtonDictionary[buttonIndex].ButtonComponent.onClick.AddListener(action);
            m_menuButtonDictionary[buttonIndex].ImageComponent.sprite = buttonImage;
            m_menuButtonDictionary[buttonIndex].TextToolTip = buttonToolTip;
            m_menuButtonDictionary[buttonIndex].HasToolTip = !string.IsNullOrEmpty(buttonToolTip);
            m_menuButtonDictionary[buttonIndex].ShortcutKey = shortcutKey;
            m_menuButtonDictionary[buttonIndex].ButtonObject.SetActive(true);
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
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework.ExtensionScripts;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.Game.MenuButtons;
using Project_Assets.Scripts.Structs;
using UnityEditor;
using UnityEngine;

namespace Project_Assets.Scripts.Player
{
    public class PlayerMenuButtons : MonoBehaviour
    {
        [SerializeField] private ObjectMenuButton[] m_objectMenuButtons;

        private GameMenuButtons m_gameMenuButtons;

        // TODO: Listen to button presses for shortcut keys
        public void Initialize()
        {
            ServiceLocator.Global.Get(out m_gameMenuButtons);
            SetGameMenuButtons(); // Player is selected at start so set buttons immediately
        }

        public void SetGameMenuButtons()
        {
            m_gameMenuButtons.ResetButtonBinds();

            foreach (var button in m_objectMenuButtons)
            {
                m_gameMenuButtons.BindButton(button.GameMenuButton, button.ClickedAction, button.Icon,
                    button.TextToolTip, button.ShortcutKey, null);
            }
        }

        public void ExitTheGame()
        {
            Debug.Log("Exiting the game".Color(Color.red));

#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}
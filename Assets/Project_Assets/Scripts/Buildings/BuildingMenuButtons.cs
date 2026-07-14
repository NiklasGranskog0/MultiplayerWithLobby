using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Game.MenuButtons;
using Project_Assets.Scripts.Structs;
using UnityEditor;
using UnityEngine;

namespace Project_Assets.Scripts.Buildings
{
    public class BuildingMenuButtons : MonoBehaviour
    {
        [SerializeField] private ObjectMenuButton[] m_objectMenuButtons;
        private GameMenuButtons m_gameMenuButtons;
        
        public void Start()
        {
            ServiceLocator.Global.Get(out m_gameMenuButtons);
        }

        public void SetGameMenuButtons()
        {
            m_gameMenuButtons.ResetButtonBinds();
            
            foreach (var button in m_objectMenuButtons)
            {
                m_gameMenuButtons.BindButton(button.GameMenuButton, button.ClickEvent.Invoke, button.Icon, button.TextToolTip, button.ShortcutKey);
            }
        }

        public void ExitTheGame()
        {
            Debug.Log("Exiting the game".Color("red"));

#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}

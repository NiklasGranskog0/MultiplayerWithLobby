using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.Structs;
using UnityEngine;

namespace Project_Assets.Scripts.Game.MenuButtons
{
    // TODO: Listen for shortcut button presses
    public class MenuButtonsBase : MonoBehaviour
    {
        [SerializeField] private ObjectMenuButton[] m_objectMenuButtons;
        private GameMenuButtons m_gameMenuButtons;

        public virtual void Initialize()
        {
            ServiceLocator.Global.Get(out m_gameMenuButtons);
        }

        public virtual void SetGameMenuButtons()
        {
            m_gameMenuButtons.ResetButtonBinds();

            foreach (var button in m_objectMenuButtons)
            {
                m_gameMenuButtons.BindButton(button.GameMenuButton, button.ClickedAction, button.Icon,
                    button.TextToolTip, button.ShortcutKey, button.Callback);
            }
        }
        
        public UnitType NullCallback() => UnitType.None;
    }
}
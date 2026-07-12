using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Game.MenuButtons;
using Project_Assets.Scripts.Interfaces;
using Project_Assets.Scripts.ScriptableObjects;
using Project_Assets.Scripts.ScriptableObjects.SerializedDictionaries;
using Project_Assets.Scripts.Structs;
using UnityEngine;

namespace Project_Assets.Scripts.Player
{
    public class PlayerMenuButtons : MonoBehaviour, IGameMenuButton
    {
        [SerializeField] private SpriteDictionary m_spriteDictionary;
        [SerializeField] private ObjectMenuButton[] m_objectMenuButtons;
        
        private GameMenuButtons m_gameMenuButtons;
        private PlayerInputs m_playerInputs;
        
        // TODO: Listen to button presses for shortcut keys
       public void Initialize(PlayerInputs playerInputs)
       {
           ServiceLocator.Global.Get(out m_gameMenuButtons);
           m_playerInputs = playerInputs;
           SetGameMenuButtons();
       }
       
       // TODO: There is only 12 buttons available, if there is more continue.
       // TODO: Only bind buttons that have a action assigned to them.
       // TODO: Make sure a button can only be bound once.
       // Currently a button can get new values if bound again. 
       public void SetGameMenuButtons()
       {
           m_gameMenuButtons.ResetButtonBinds();

           foreach (var button in m_objectMenuButtons)
           {
               m_gameMenuButtons.BindButton(button.GameMenuButton, button.ClickEvent.Invoke, button.Icon, button.TextToolTip, button.ShortcutKey);
           }
       }
       
       public void OnButtonTest()
       {
           Action();
       }

       private void Action()
       {
           Debug.Log("Action".Color("orange"));
       }
    }
}

using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Game.MenuButtons;
using Project_Assets.Scripts.Interfaces;
using Project_Assets.Scripts.ScriptableObjects;
using Project_Assets.Scripts.ScriptableObjects.SerializedDictionaries;
using UnityEngine;

namespace Project_Assets.Scripts.Player
{
    public class PlayerMenuButtons : MonoBehaviour, IGameMenuButton
    {
        [SerializeField] private SpriteDictionary m_spriteDictionary; 
        private GameMenuButtons m_gameMenuButtons;
        private PlayerInputs m_playerInputs;
        
        // TODO: Listen to button presses for shortcut keys
       public void Initialize(PlayerInputs playerInputs)
       {
           ServiceLocator.Global.Get(out m_gameMenuButtons);
           m_playerInputs = playerInputs;
           SetGameMenuButtons();
       }

       private void Action()
       {
           Debug.Log("Action".Color("orange"));
       }

       public void SetGameMenuButtons()
       {
           m_gameMenuButtons.ResetButtonBinds();
           
           m_gameMenuButtons.BindButton(GameMenuButton.TopLeft, Action, m_spriteDictionary["moss_blue12"], "Tooltip", KeyCode.A);
       }
    }
}

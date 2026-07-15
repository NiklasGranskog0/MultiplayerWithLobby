using Project_Assets.Scripts.Framework.ExtensionScripts;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.Game;
using Project_Assets.Scripts.Game.MenuButtons;
using Project_Assets.Scripts.ScriptableObjects;
using Project_Assets.Scripts.Structs;
using UnityEngine;

namespace Project_Assets.Scripts.Player
{
    public class PlayerMenuButtons : MonoBehaviour
    {
        [SerializeField] private ObjectMenuButton[] m_objectMenuButtons;
        
        private GameMenuButtons m_gameMenuButtons;
        private GameManager m_gameManager;
        private PlayerInputs m_playerInputs; // for listening to button presses probably
        
        private ulong m_clientId;
        private string m_teamTag;
        
        // TODO: Listen to button presses for shortcut keys
       public void Initialize(PlayerInputs playerInputs, ulong clientId, string teamTag)
       {
           ServiceLocator.Global.Get(out m_gameMenuButtons);
           ServiceLocator.Global.Get(out m_gameManager);
           m_clientId = clientId;
           m_teamTag = teamTag;
           
           m_playerInputs = playerInputs;
           SetGameMenuButtons(); // Temp, player is selected at start so set buttons immediately
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

       // TODO: Test func, real function should take in a "enum to prefab(enum)" parameter
       public void SpawnTestPrefab()
       {
           m_gameManager.SpawnTestPrefab(m_clientId, m_teamTag);
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

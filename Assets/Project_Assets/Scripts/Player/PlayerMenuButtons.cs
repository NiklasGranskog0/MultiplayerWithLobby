using Project_Assets.Scripts.Game.MenuButtons;
using Project_Assets.Scripts.UtilityExtensions.Strings;
using UnityEditor;
using UnityEngine;

namespace Project_Assets.Scripts.Player
{
    public class PlayerMenuButtons : MenuButtonsBase
    {
        // Temp to test button call
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
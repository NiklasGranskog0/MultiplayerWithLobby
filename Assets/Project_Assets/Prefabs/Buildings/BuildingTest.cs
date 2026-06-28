using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Game.MenuButtons;
using Project_Assets.Scripts.Interfaces;
using UnityEngine;

namespace Project_Assets.Prefabs.Buildings
{
    public class BuildingTest : MonoBehaviour, ISelectionObject, IGameMenuButton
    {
        private GameMenuButtons m_gameMenuButtons;
        public ImageToLoad ImageToLoad => ImageToLoad.BaseTier1;
        public string Name => "BuildingTest";

        private void Awake()
        {
            ServiceLocator.Global.Get(out m_gameMenuButtons);
        }

        public void SetGameMenuButtons()
        {
            m_gameMenuButtons.ResetButtonBinds();
        }
    }
}

using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Interfaces;
using UnityEngine;

namespace Project_Assets.Scripts.Buildings
{
    // TODO: Rename to Castle or something
    public class BaseTierOne : MonoBehaviour, ISelectionObject
    {
        [SerializeField] private BuildingMenuButtons m_buildingMenuButtons;
        
        public ImageToLoad ImageToLoad => ImageToLoad.Castle;
        public string Name => "Castle";
        public void SetGameMenuButtons()
        {
            m_buildingMenuButtons.SetGameMenuButtons();
        }
    }
}

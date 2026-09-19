using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Game.MenuButtons;
using Project_Assets.Scripts.Network.Game;
using UnityEngine;

namespace Project_Assets.Scripts.Buildings
{
    public class BuildingMenuButtons : MenuButtonsBase
    {
        [SerializeField] private Transform m_spawnPosition;
        
        public UnitType SpawnUnit(UnitType unitType)
        {
            PoolManager.Instance.SpawnPooledObject(unitType, m_spawnPosition.position, gameObject.tag);
            return unitType; // Redundant, but callback needs to have a return value.
        }
    }
}
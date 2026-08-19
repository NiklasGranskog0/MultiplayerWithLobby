using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.Game.MenuButtons;
using Project_Assets.Scripts.Network;
using Project_Assets.Scripts.Structs;
using UnityEngine;

namespace Project_Assets.Scripts.Buildings
{
    public class BuildingMenuButtons : MonoBehaviour
    {
        [SerializeField] private Transform m_spawnPosition;
        [SerializeField] private ObjectMenuButton[] m_objectMenuButtons;
        private GameMenuButtons m_gameMenuButtons;
        private PoolManager m_poolManager;

        public void Initialize()
        {
            ServiceLocator.Global.Get(out m_gameMenuButtons);
            // ServiceLocator.ForSceneOf(this).Get(out m_poolManager);
        }

        public void SetGameMenuButtons()
        {
            m_gameMenuButtons.ResetButtonBinds();

            // because we have a separate callback to spawn units, we can essentially run 2 methods on the same button
            foreach (var button in m_objectMenuButtons)
            {
                m_gameMenuButtons.BindButton(button.GameMenuButton, button.ClickedAction, button.Icon,
                    button.TextToolTip, button.ShortcutKey, button.Callback);
            }
        }
        
        public UnitType CallbackNull() => UnitType.None;
        
        public UnitType SpawnUnit(UnitType unitType)
        {
            if (m_poolManager is null) ServiceLocator.ForSceneOf(this).Get(out m_poolManager);
                
            m_poolManager.SpawnPooledObject(unitType, m_spawnPosition.position, gameObject.tag);
            return unitType; // Redundant, but callback needs to have a return value.
        }
    }
}
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.Game.MenuButtons;

namespace Project_Assets.Scripts.Units.Types
{
    public class ManSoldierFullPlateFantasyA : UnitBase
    {
        private GameMenuButtons m_gameMenuButtons;

        public override void Start()
        {
            base.Start();
            ServiceLocator.Global.Get(out m_gameMenuButtons);
        }
        
        public override ImageToLoad ImageToLoad => ImageToLoad.ManSoldierFullPlateFantasyA;
        public override string Name => "Soldier";
        public override void SetGameMenuButtons() => m_gameMenuButtons.ResetButtonBinds();
    }
}

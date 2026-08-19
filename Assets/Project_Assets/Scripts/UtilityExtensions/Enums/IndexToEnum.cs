using System;
using Project_Assets.Scripts.Enums;

namespace Project_Assets.Scripts.UtilityExtensions.Enums
{
    public static class IndexToEnum
    {
        public static GameSpeed GameSpeedFromIndex(this int index)
        {
            return index switch
            {
                0 => GameSpeed.Slow,
                1 => GameSpeed.Normal,
                2 => GameSpeed.Fast,
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };
        }

        public static GameMode GameModeFromIndex(this int index)
        {
            return index switch
            {
                0 => GameMode.CaptureTheFlag,
                1 => GameMode.TeamDeathMatch,
                2 => GameMode.BattleRoyal,
                3 => GameMode.SuddenDeath,
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };
        }
    }
}
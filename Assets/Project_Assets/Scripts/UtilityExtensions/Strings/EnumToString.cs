using System;
using Project_Assets.Scripts.Enums;

namespace Project_Assets.Scripts.UtilityExtensions.Strings
{
    public static class EnumToString
    {
        public static string GameSpeedToString(this GameSpeed speed)
        {
            return speed switch
            {
                GameSpeed.Slow => "Slow",
                GameSpeed.Normal => "Normal",
                GameSpeed.Fast => "Fast",
                _ => throw new ArgumentOutOfRangeException(nameof(speed), speed, null)
            };
        }
        
        public static string GameModeToString(this GameMode mode)
        {
            return mode switch
            {
                GameMode.CaptureTheFlag => "Capture The Flag",
                GameMode.TeamDeathMatch => "Team Death Match",
                GameMode.BattleRoyal => "Battle Royal",
                GameMode.SuddenDeath => "Sudden Death",
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        public static string GameMapToString(this Map map)
        {
            return map switch
            {
                Map.Maze => "Maze",
                Map.Forest => "Forest",
                Map.Desert => "Desert",
                Map.Mountain => "Mountain",
                Map.River => "River",
                _ => throw new ArgumentOutOfRangeException(nameof(map), map, null)
            };
        }
    }
}
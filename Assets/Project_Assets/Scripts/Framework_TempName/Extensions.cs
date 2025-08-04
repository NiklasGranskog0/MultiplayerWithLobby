using System;
using Project_Assets.Scripts.Enums;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project_Assets.Scripts.Framework_TempName
{
    public static class Extensions
    {
        #region GameObjects

        public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;

        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.TryGetComponent<T>(out var component) ? component : gameObject.AddComponent<T>();
        }

        #endregion

        #region color

        public static string Color(this string s, string color) => $"<color={color.ToUpper()}>{s}</color>";

        #endregion
        
        #region Enums

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

        #endregion
    }
}
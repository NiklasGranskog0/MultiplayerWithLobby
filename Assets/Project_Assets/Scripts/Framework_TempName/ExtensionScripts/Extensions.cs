using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project_Assets.Scripts.Enums;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Project_Assets.Scripts.Framework_TempName.ExtensionScripts
{
    public static class Extensions
    {
        #region GameObjects

        public static T OrNull<T>(this T obj) where T : UnityEngine.Object => obj ? obj : null;

        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.TryGetComponent<T>(out var component) ? component : gameObject.AddComponent<T>();
        }

        #endregion
        
        #region Transform
        
        public static void ClearContainer(this Transform container)
        {
            foreach (Transform child in container)
            {
                if (child != null)
                {
                    UnityEngine.Object.Destroy(child.gameObject);
                }
            }
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

        #endregion

        #region Strings

        public static IEnumerator FadeOut(this TMP_Text t, float fadeDuration)
        {
            var duration = 0f;

            while (duration < fadeDuration)
            {
                var alpha = Mathf.Lerp(1f, 0f, duration / fadeDuration);
                t.color = new Color(t.color.r, t.color.g, t.color.b, alpha);
                duration += Time.deltaTime;
                yield return null;
            }
        }

        #endregion

        #region Lobby

        public static UpdatePlayerOptions UpdatePlayerData(string key, string value,
            PlayerDataObject.VisibilityOptions visibility = PlayerDataObject.VisibilityOptions.Public)
        {
            return new UpdatePlayerOptions()
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {
                        key, new PlayerDataObject(visibility, value)
                    }
                }
            };
        }

        public static Task<Unity.Services.Lobbies.Models.Lobby> UpdateLobbyData(string lobbyId, string key,
            string value, DataObject.VisibilityOptions visibility = DataObject.VisibilityOptions.Member)
        {
            return LobbyService.Instance.UpdateLobbyAsync(lobbyId, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {
                        key, new DataObject(visibility, value)
                    }
                }
            });
        }

        #endregion

        #region NetworkGameObjects

        public static NetworkObject CreateNetworkObject(GameObject prefab, Transform position, ulong clientId,
            bool isPlayerObject = false, bool destroyWithScene = false, bool forceOverride = false)
        {
            if (!prefab.TryGetComponent<NetworkObject>(out var networkObject))
            {
                Debug.LogError($"NetworkObject not found on prefab: {prefab.name}");
                return null;
            }
            
            return NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(
                networkObject,
                clientId,
                destroyWithScene,
                isPlayerObject,
                forceOverride,
                position.position);
        }

        #endregion
    }
}
using System.Collections.Generic;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework_TempName;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Structs
{
    /// <summary>
    /// The player creating the lobby is stored in Player
    /// </summary>
    public struct CreateLobbySettings
    {
        public Player Player { get; set; }
        public bool? IsPrivate { get; set; }
        public bool? IsLocked { get; set; }
        public Image GameImage { get; set; }
        public (GameMode mode, DataObject.VisibilityOptions visibilityOptions) GameMode { get; set; }
        public (Map map, DataObject.VisibilityOptions visibilityOptions) Map { get; set; }
        public (int max, DataObject.VisibilityOptions visibilityOptions) MaxPlayers { get; set; }
        public (string name, DataObject.VisibilityOptions visibilityOptions) GameName { get; set; }
        public (GameSpeed speed, DataObject.VisibilityOptions visibilityOptions) GameSpeed { get; set; }
        public Dictionary<string, DataObject> Data { get; private set; }

        public void SetData()
        {
            Data = new Dictionary<string, DataObject>
            {
                {KeyConstants.k_GameMode, new DataObject(GameMode.visibilityOptions, GameMode.mode.GameModeToString())},
                {KeyConstants.k_Map, new DataObject(Map.visibilityOptions, Map.map.ToString())},
                {KeyConstants.k_MaxPlayers, new DataObject(MaxPlayers.visibilityOptions, MaxPlayers.max.ToString())},
                {KeyConstants.k_GameName, new DataObject(GameName.visibilityOptions, GameName.name)},
                {KeyConstants.k_GameSpeed, new DataObject(GameSpeed.visibilityOptions, GameSpeed.speed.GameSpeedToString())},
            };
        }
    }
}

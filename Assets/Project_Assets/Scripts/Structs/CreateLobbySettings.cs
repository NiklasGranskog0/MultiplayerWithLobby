using System.Collections.Generic;
using System.Text;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework_TempName;
using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Unity.Services.Lobbies.Models;
using UnityEngine;
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
        public string Password { get; set; }
        public (GameMode mode, DataObject.VisibilityOptions visibilityOptions) GameMode { get; set; }
        public (Map map, DataObject.VisibilityOptions visibilityOptions) GameMap { get; set; }
        public (int max, DataObject.VisibilityOptions visibilityOptions) MaxPlayers { get; set; }
        public (string name, DataObject.VisibilityOptions visibilityOptions) GameName { get; set; }
        public (GameSpeed speed, DataObject.VisibilityOptions visibilityOptions) GameSpeed { get; set; }
        public (RawImage image, string imageName, DataObject.VisibilityOptions visibilityOptions) GameImage { get; set; }
        public Dictionary<string, DataObject> Data { get; private set; }

        public void SetData()
        {
            Data = new Dictionary<string, DataObject>
            {
                {KeyConstants.k_GameMode, new DataObject(GameMode.visibilityOptions, GameMode.mode.GameModeToString())},
                {KeyConstants.k_Map, new DataObject(GameMap.visibilityOptions, GameMap.map.GameMapToString())},
                {KeyConstants.k_MaxPlayers, new DataObject(MaxPlayers.visibilityOptions, MaxPlayers.max.ToString())},
                {KeyConstants.k_GameName, new DataObject(GameName.visibilityOptions, GameName.name)},
                {KeyConstants.k_GameSpeed, new DataObject(GameSpeed.visibilityOptions, GameSpeed.speed.GameSpeedToString())},
                {KeyConstants.k_GameImage, new DataObject(DataObject.VisibilityOptions.Public, GameImage.imageName)},
            };
        }
    }
}

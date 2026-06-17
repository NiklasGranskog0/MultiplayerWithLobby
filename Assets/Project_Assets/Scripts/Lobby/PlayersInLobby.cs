using System.Collections.Generic;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using UnityEngine;

namespace Project_Assets.Scripts.Lobby
{
    public class PlayersInLobby : MonoBehaviour
    {
        public Dictionary<string, Unity.Services.Lobbies.Models.Player> Players;
        
        private void Awake()
        {
            Players = new Dictionary<string, Unity.Services.Lobbies.Models.Player>();
            ServiceLocator.Global.Register(this, ServiceLevel.Global, gameObject.scene.name);
        }

        private void Start()
        {
            // this = lobby scene
            ServiceLocator.ForSceneOf(this).Get(out LobbyManager lobbyManager);
            lobbyManager.OnSendLobbyPlayers += OnSendLobbyPlayers;
        }
        
        private void OnSendLobbyPlayers(Unity.Services.Lobbies.Models.Lobby lobby)
            => lobby.Players.ForEach(player => Players.Add(player.Id, player));
        
    }
}

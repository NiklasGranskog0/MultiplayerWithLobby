using System.Collections.Generic;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using UnityEngine;

namespace Project_Assets.Scripts.Lobby
{
    /*TODO public class PlayersInLobby
    // {
    //     public Dictionary<string, Unity.Services.Lobbies.Models.Player> Players;
    //     private LobbyManager m_lobbyManager;
    //
    // public PlayersInLobby(LobbyManager lobbyManager)
    // {
    //     Initialize();
    // }
    //     private void Initialize()
    //     {
    //         Players = new Dictionary<string, Unity.Services.Lobbies.Models.Player>();
    //         ServiceLocator.Global.Register(this, ServiceLevel.Global);
    //     }
    } */
    
    // TODO: No real need to have this class as a MonoBehaviour
    // Class to keep track of players in lobby that is connecting to the game
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

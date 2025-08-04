using System;
using System.Collections.Generic;
using Project_Assets.Scripts.Structs;

namespace Project_Assets.Scripts.Events
{
    public class LobbyEventArgs : EventArgs
    {
        public Unity.Services.Lobbies.Models.Lobby Lobby;
    }

    public class LobbyListChangedEventArgs : EventArgs
    {
        public List<Unity.Services.Lobbies.Models.Lobby> Lobbies;
    }
}
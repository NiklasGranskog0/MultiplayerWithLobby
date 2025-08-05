using System.Collections.Generic;
using Project_Assets.Scripts.Framework_TempName;
using Project_Assets.Scripts.Interfaces;
using UnityEngine;

namespace Project_Assets.Scripts.Structs
{
    public struct StatusReport : ILog
    {
        public string Message;
        public bool Success;
        
        public void MakeReport(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public void Log()
        {
            Debug.Log($"Success: {Success} :  Message: {Message}".Color(Success ? "green" : "red"));
        }
    }

    public struct LobbyStatusReport : ILog
    {
        public Unity.Services.Lobbies.Models.Lobby Lobby;
        private StatusReport m_Status;
        
        public void MakeReport(Unity.Services.Lobbies.Models.Lobby lobby, bool success, string message)
        {
            Lobby = lobby;
            m_Status.Message = message;
            m_Status.Success = success;
        }

        public void Log()
        {
            Debug.Log($"Success: {m_Status.Success} :  Message: {m_Status.Message}".Color(m_Status.Success ? "green" : "red"));
        }
    }

    public struct LobbiesStatusReport : ILog
    {
        public List<Unity.Services.Lobbies.Models.Lobby> Lobbies;
        public StatusReport Status;

        public void MakeReport(List<Unity.Services.Lobbies.Models.Lobby> lobbies, bool success, string message)
        {
            Lobbies = lobbies;
            Status.Message = message;
            Status.Success = success;
        }

        public void Log()
        {
            Debug.Log($"Success: {Status.Success} :  Message: {Status.Message}".Color(Status.Success ? "green" : "red"));
        }
    }
}

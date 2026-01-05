using System;
using System.Threading.Tasks;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Lobby;
using Project_Assets.Scripts.Structs;
using Unity.Services.Vivox;
using UnityEngine;

namespace Project_Assets.Scripts.TextChat
{
    public class VivoxManager : MonoBehaviour
    {
        public string CurrentChannelName { get; private set; }
        private static StatusReport s_statusReport;
        
        private LobbyManager m_lobbyManager;

        private void Awake()
        {
            ServiceLocator.Global.Register(this, ServiceLevel.Global);
        }

        private void Start()
        {
            ServiceLocator.Global.Get(out m_lobbyManager);

            m_lobbyManager.OnJoinedTextChannel += OnJoinedLobbyChannel;
            m_lobbyManager.OnLeftTextChannel += OnLeftLobbyChannel;
        }

        private async void OnLeftLobbyChannel(string obj)
        {
            var report = await LeaveLobbyChannel(); 
            report.Log();
            
            CurrentChannelName = string.Empty;
        }

        private async void OnJoinedLobbyChannel(string obj)
        {
            CurrentChannelName = obj;
            
            var report = await JoinLobbyChannel();
            report.Log();
        }

        private async Task<StatusReport> JoinLobbyChannel()
        {
            try
            {
                await VivoxService.Instance.JoinGroupChannelAsync(CurrentChannelName, ChatCapability.TextOnly);
                s_statusReport.MakeReport(true, $"Joined Text Channel: {CurrentChannelName}");
            }
            catch (Exception e)
            {
                s_statusReport.MakeReport(false,
                    $"Failed to join Text Channel Name: {CurrentChannelName}, Message: {e.Message}");
            }

            return s_statusReport;
        }

        private async Task<StatusReport> LeaveLobbyChannel()
        {
            try
            {
                await VivoxService.Instance.LeaveChannelAsync(CurrentChannelName);
                s_statusReport.MakeReport(true, $"Left Text Channel: {CurrentChannelName}");
            }
            catch (Exception e)
            {
                s_statusReport.MakeReport(false, 
                    $"Failed to leave Text Channel Name: {CurrentChannelName},  Message: {e.Message}");
            }

            return s_statusReport;
        }
    }
}
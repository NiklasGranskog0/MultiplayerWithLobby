using System;
using System.Threading.Tasks;
using Project_Assets.Scripts.Framework_TempName;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.Structs;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Project_Assets.Scripts.Network.Relay
{
    public class RelayManager : MonoBehaviour
    {
        private static RelayStatus s_statusReport;
        
        private void Awake() => ServiceLocator.Global.Register(this, ServiceLevel.Global);
        
        public async Task<RelayStatus> CreateRelay(int maxPlayers)
        {
            try
            {
                var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
                var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                
                var relayServerData = allocation.ToRelayServerData("dtls");

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                NetworkManager.Singleton.StartHost();
                
                s_statusReport.JoinCode = joinCode;
                s_statusReport.MakeReport(true, $"Relay Created, Join Code: {joinCode}");
            }
            catch (RelayServiceException e)
            {
                s_statusReport.MakeReport(false, $"Create Relay failed: {e.Message}");
            }

            return s_statusReport;
        }

        public static async Task<RelayStatus> JoinRelay(string joinCode)
        {
            try
            {
                var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                var relayServerData = allocation.ToRelayServerData("dtls");
                
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                NetworkManager.Singleton.StartClient();
                
                s_statusReport.MakeReport(true, $"Joined relay with Code: {joinCode}");
            }
            catch (RelayServiceException e)
            {
                s_statusReport.MakeReport(false, $"Join Relay failed: {e.Message}");
            }
            
            return s_statusReport;
        }
    }
}

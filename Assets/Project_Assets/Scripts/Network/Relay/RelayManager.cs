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
                
                s_statusReport.JoinCode = joinCode;
                s_statusReport.MakeReport(true, $"Relay Created, Join Code: {joinCode}");
            }
            catch (Exception e)
            {
                s_statusReport.MakeReport(false, $"Create Relay failed: {e.Message}");
            }

            return s_statusReport;
        }
    }
}

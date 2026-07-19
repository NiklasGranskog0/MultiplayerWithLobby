using System;
using Project_Assets.Scripts.Framework.ExtensionScripts;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.Lobby;
using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.Game
{
    [Serializable]
    public struct PlayerObjects
    {
        public GameObject PlayerPrefab;
        public GameObject PlayerCameraPrefab;
    }
    
    public class PlayerSpawnManager : MonoBehaviour
    {
        [SerializeField] private Transform[] m_spawnPoints;
        [SerializeField] private PlayerObjects m_playerObjects;
        
        private PlayersInLobby m_playersInLobby;

        private void Start()
        {
            if (!NetworkManager.Singleton.IsHost) return;
            
            ServiceLocator.Global.Get(out m_playersInLobby);
            CreateAndSpawnPlayers();
        }
        
        private void CreateAndSpawnPlayers()
        {
            // Creates a player and camera network object for each player in the lobby
            foreach (var player in m_playersInLobby.Players)
            {
                var data = player.Value.Data;
                var id = ulong.Parse(data[StringConstants.k_PlayerClientId].Value);
                var teamNb = ulong.Parse(data[StringConstants.k_PlayerTeam].Value);
                
                var playerObj = Extensions.CreateNetworkObject(m_playerObjects.PlayerPrefab,  m_spawnPoints[teamNb], id);
                var playerCam = Extensions.CreateNetworkObject(m_playerObjects.PlayerCameraPrefab,  m_spawnPoints[teamNb], id);
                
                // Set the player's and camera tag to their team number
                playerObj.gameObject.tag = Enum.GetName(typeof(Enums.Team), teamNb);
                playerCam.gameObject.tag = Enum.GetName(typeof(Enums.Team), teamNb);
            }
        }
    }
}

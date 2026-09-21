using System.Collections.Generic;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using Project_Assets.Scripts.Game;
using Project_Assets.Scripts.Lobby;
using Project_Assets.Scripts.Player;
using Project_Assets.Scripts.UtilityExtensions.GlobalConstants.Strings;
using Project_Assets.Scripts.UtilityExtensions.NetworkExtensions;
using Project_Assets.Scripts.UtilityExtensions.Strings;
using Unity.Netcode;
using UnityEngine;

namespace Project_Assets.Scripts.Network.Game
{
    public class PlayerCreator : NetworkBehaviour
    {
        // TODO: Only need player prefab
        [SerializeField] private GameObject m_playerPrefab;
        private IReadOnlyList<Transform> m_spawnPoints;
        private PlayersInLobby m_playersInLobby;

        public override void OnNetworkSpawn()
        {
            Debug.Log("Player Creator OnNetworkSpawn".Color(Color.lightSalmon));
            m_spawnPoints = SpawnPoints.Instance.Points;

            // This is a list of all the players in the lobby, object will be null when lobby scene is unloaded.
            ServiceLocator.Global.Get(out m_playersInLobby);
        }

        public void CreatePlayers()
        {
            Debug.Log("Player Creator CreatePlayers".Color(Color.lightSalmon));
            SpawnPlayers();
        }

        private void SpawnPlayers()
        {
            foreach (var player in m_playersInLobby.Players)
            {
                var data = player.Value.Data;
                var playerId = ulong.Parse(data[StringConstants.k_PlayerClientId].Value);
                var teamNumber = int.Parse(data[StringConstants.k_PlayerTeam].Value); // 0 == team 1, 1 == team 2

                Debug.Log(
                    $"Player Creator SpawnPlayers: PlayerId: {playerId}, TeamNumber: {teamNumber}, " +
                    $"SpawnPoint: {m_spawnPoints[teamNumber]}, Team: {(Enums.Team)teamNumber}");

                var playerNetworkObject = m_playerPrefab.CreateAsNetworkObjectAndSpawn(
                    m_spawnPoints[teamNumber].position,
                    playerId);

                Debug.Log($"Player Creator Setting Team Tag: {(Enums.Team)teamNumber}");
                var playerObject = playerNetworkObject.GetComponent<PlayerCharacterBase>();
                playerObject.PlayerTeam.Value = (Enums.Team)teamNumber;
            }

            Debug.Log("Player Creator SpawnPlayers: Done".Color(Color.lightSalmon));
        }
    }
}
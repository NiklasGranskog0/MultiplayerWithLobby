using System.Collections.Generic;
using System.Threading.Tasks;
using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project_Assets.Scripts.Authentication
{
    public class PlayerAuthentication : MonoBehaviour
    {
        public Player Player;
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }

        private async void Awake()
        {
            ServiceLocator.Global.Register(this, ServiceLevel.Global);
            await AuthenticatePlayer();
        }

        private async Task AuthenticatePlayer()
        {
            await Authenticate("Player_" + Random.Range(0, 1000));
            Player = CreateLocalPlayer();
        }

        private async Task Authenticate(string playerName)
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                InitializationOptions options = new InitializationOptions();
                options.SetProfile(playerName);

                await UnityServices.InitializeAsync(options);
            }

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in as " + AuthenticationService.Instance.PlayerId);
            };

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                PlayerId = AuthenticationService.Instance.PlayerId;
                PlayerName = playerName;
            }
        }

        // Returns a Player object with metadata.
        private Player CreateLocalPlayer()
        {
            return new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {
                        KeyConstants.k_PlayerName, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,
                            AuthenticationService.Instance.Profile)
                    },

                    {
                        KeyConstants.k_PlayerId, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,
                            AuthenticationService.Instance.PlayerId)
                    },
                    
                    { 
                        KeyConstants.k_PlayerTeam, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "0")  
                    },

                    {
                       KeyConstants.k_PlayerReady, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "false")  
                    },
                }
            };
        }
    }
}
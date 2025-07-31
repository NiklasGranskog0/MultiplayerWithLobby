using System.Threading.Tasks;
using Project_Assets.Scripts.Framework_TempName;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project_Assets.Scripts.Authentication
{
    public class PlayerAuthentication : Singleton<PlayerAuthentication>
    {
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }

        private async void Awake()
        {
            await AuthenticatePlayer();
        }

        private async Task AuthenticatePlayer()
        {
            await Authenticate("Player_" + Random.Range(0, 1000));
        }

        private async Task Authenticate(string playerName)
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                InitializationOptions options = new InitializationOptions();
                options.SetProfile(playerName);
                
                Debug.Log(playerName);
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
    }
}

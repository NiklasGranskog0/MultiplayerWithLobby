using System.Threading.Tasks;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Unity.Services.Authentication;
using Unity.Services.Vivox;
using UnityEngine;

namespace Project_Assets.Scripts.TextChat
{
    public class VivoxAuthentication : MonoBehaviour
    {
        private void Awake()
        {
            ServiceLocator.Global.Register(this, ServiceLevel.Global);
        }

        private async void Start()
        {
            await AuthenticateVivox();
        }

        private async Task AuthenticateVivox()
        {
            await VivoxService.Instance.InitializeAsync(new VivoxConfigurationOptions { });
            await LoginToVivox(AuthenticationService.Instance.Profile);
        }
        
        private async Task LoginToVivox(string displayName)
        {
            LoginOptions options = new LoginOptions
            {
                DisplayName = displayName,
                EnableTTS = false,
            };
            
            await VivoxService.Instance.LoginAsync(options);
        }
    }
}
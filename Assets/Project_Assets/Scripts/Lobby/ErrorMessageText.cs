using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using TMPro;
using UnityEngine;

namespace Project_Assets.Scripts.Lobby
{
    public class ErrorMessageText : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        
        private void Awake()
        {
            ServiceLocator.Global.Register(this, ServiceLevel.Global);
        }

        public void SetText(string errorMessage)
        {
            text.text = errorMessage;
            FadeOut();
        }

        // TODO: Fade out text
        private void FadeOut()
        {
            
        }
    }
}

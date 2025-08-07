using System;
using TMPro;

namespace Project_Assets.Scripts.Structs
{
    [Serializable]
    public struct ErrorPanels
    {
        public TMP_Text errorText;
        public float fadeDuration;

        public TMP_Text SetText(string errorMessage)
        {
            errorText.text = errorMessage;
            return errorText;
        }
    }
}

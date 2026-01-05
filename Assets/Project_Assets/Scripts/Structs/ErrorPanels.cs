using System;
using TMPro;

namespace Project_Assets.Scripts.Structs
{
    [Serializable]
    public struct ErrorPanels
    {
        public TMP_Text ErrorText;
        public float FadeDuration;

        public TMP_Text SetText(string errorMessage)
        {
            ErrorText.text = errorMessage;
            return ErrorText;
        }
    }
}

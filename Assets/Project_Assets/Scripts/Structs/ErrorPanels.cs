using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Project_Assets.Scripts.Structs
{
    [Serializable]
    public struct ErrorPanels
    {
        public TMP_Text errorText;
        public float fadeDuration;

        public void SetText(string errorMessage)
        {
            errorText.text = errorMessage;
        }
        
        private IEnumerator FadeOut()
        {
            var duration = 0f;
            
            while (duration < fadeDuration)
            {
                var alpha = Mathf.Lerp(1f, 0f, duration / fadeDuration);
                errorText.color = new Color(errorText.color.r, errorText.color.g, errorText.color.b, alpha);
                duration += Time.deltaTime;
                yield return null;
            }
        }
    }
}

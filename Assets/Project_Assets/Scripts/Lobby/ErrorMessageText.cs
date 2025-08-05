using System.Collections;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using TMPro;
using UnityEngine;

namespace Project_Assets.Scripts.Lobby
{
    public class ErrorMessageText : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private float fadeOutDuration;
        private float m_Duration;
        
        private void Awake()
        {
            ServiceLocator.Global.Register(this, ServiceLevel.Global);
        }

        public void SetText(string errorMessage)
        {
            text.text = errorMessage;
            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut()
        {
            m_Duration = 0f;
            
            while (m_Duration < fadeOutDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, m_Duration / fadeOutDuration);
                text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
                m_Duration += Time.deltaTime;
                yield return null;
            }
        }
    }
}

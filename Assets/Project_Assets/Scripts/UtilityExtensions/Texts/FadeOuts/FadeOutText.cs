using System.Collections;
using TMPro;
using UnityEngine;

namespace Project_Assets.Scripts.UtilityExtensions.Texts.FadeOuts
{
    public static class FadeOutText
    {
        // TODO: Unity boss room has an example of fade out for scenes
        public static IEnumerator FadeOut(this TMP_Text t, float fadeDuration)
        {
            var duration = 0f;

            while (duration < fadeDuration)
            {
                var alpha = Mathf.Lerp(1f, 0f, duration / fadeDuration);
                t.color = new Color(t.color.r, t.color.g, t.color.b, alpha);
                duration += Time.deltaTime;
                yield return null;
            }
        }
    }
}
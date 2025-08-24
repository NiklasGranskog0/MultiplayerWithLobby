using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Structs
{
    [System.Serializable]
    public struct LoadingScene
    {
        public GameObject loadingScreen;
        public Slider progressSlider;
        public float fillSpeed;
        public TMP_Text loadingText;
        public TMP_Text loadingTitleText;
    }
}
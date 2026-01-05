using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Scenes
{
    [Serializable]
    public class LoadingScene
    {
        public GameObject LoadingScreen;
        public Slider ProgressSlider;
        public float FillSpeed;
        public TMP_Text LoadingText;
        public TMP_Text LoadingTitleText;
    }
}
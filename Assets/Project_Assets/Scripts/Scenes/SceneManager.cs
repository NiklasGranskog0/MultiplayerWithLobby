using System;
using System.Linq;
using System.Threading.Tasks;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.ScriptableObjects;
using UnityEngine;

namespace Project_Assets.Scripts.Scenes
{
    public class SceneManager : MonoBehaviour
    {
        private LoadingProgress m_loadingProgress;
        private SceneGroupManager m_sceneGroupManager;

        [SerializeField] private LoadingScene m_defaultLoadingScreen;
        [SerializeField] private LoadingScene m_gameLoadingScreen;
        [SerializeField] private SceneGroupAsset m_sceneGroupAssets;
        [SerializeField] private SceneGroupToLoad m_sceneGroupToLoad;

        private LoadingScene m_currentLoadingScreen { get; set; }

        private const float k_targetProgress = 1f;
        private bool m_isLoading { get; set; }

        private void Awake()
        {
            m_currentLoadingScreen = m_defaultLoadingScreen;

            ServiceLocator.Global.Register(this, ServiceLevel.Global);

            m_loadingProgress = new LoadingProgress();
            m_sceneGroupManager = new SceneGroupManager();

            m_sceneGroupManager.OnSceneLoaded +=
                sceneName => Debug.Log("SceneGroupManager OnSceneLoaded: " + sceneName);

            m_sceneGroupManager.OnSceneUnloaded +=
                sceneName => Debug.Log("SceneGroupManager OnSceneUnloaded: " + sceneName);

            m_sceneGroupManager.OnSceneGroupLoaded += FinishedLoading;
        }

        private async void Start()
        {
            m_loadingProgress.OnProgressChanged += ProgressReport;
            await LoadSceneGroupByEnum(m_sceneGroupToLoad);
        }

        private void Update()
        {
            if (!m_isLoading) return;

            UpdateLoadingProgress(m_currentLoadingScreen);
        }

        public async Task LoadSceneGroupByEnum(SceneGroupToLoad sceneGroup)
        {
            ResetProgressSlider(m_currentLoadingScreen);

            Debug.Log("SceneLoader: ".Color("red") + $"Loading scene group {sceneGroup.ToString()}".Color("red"));
            EnableLoadingCanvas(true);

            await m_sceneGroupManager.LoadScenes(m_sceneGroupAssets.SceneGroups[(int)sceneGroup], m_loadingProgress);
        }

        private void UpdateLoadingProgress(LoadingScene loadingScene)
        {
            var currentFillAmount = loadingScene.ProgressSlider.value;

            loadingScene.ProgressSlider.value = Mathf.Lerp(currentFillAmount, k_targetProgress,
                Time.deltaTime * loadingScene.FillSpeed);

            loadingScene.LoadingText.text = $"{(int)(loadingScene.ProgressSlider.value * 100)}%";
        }

        private void ResetProgressSlider(LoadingScene loadingScene)
        {
            loadingScene.ProgressSlider.value = 0f;
        }

        private void ProgressReport(float value) { }

        private void FinishedLoading()
        {
            // TODO: Send message to the host that scene have been loaded

            EnableLoadingCanvas(false);
            Debug.Log("SceneLoader: ".Color("red") + "Finished Loading Scene Group".Color("lightblue"));
        }

        private void EnableLoadingCanvas(bool enable)
        {
            m_isLoading = enable;
            m_currentLoadingScreen.LoadingScreen.SetActive(enable);
        }

        public void SwitchLoadingScreen(LoadingScreenEnum loadingScreenEnum)
        {
            m_currentLoadingScreen = loadingScreenEnum switch
            {
                LoadingScreenEnum.Default => m_defaultLoadingScreen,
                LoadingScreenEnum.Game => m_gameLoadingScreen,
                _ => throw new ArgumentOutOfRangeException(nameof(loadingScreenEnum), loadingScreenEnum, null)
            };
        }

        public void SetLoadingScreenTitle(string title)
        {
            m_currentLoadingScreen.LoadingText.text = title;
        }
    }
}
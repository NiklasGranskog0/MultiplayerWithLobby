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
        private LoadingProgress m_LoadingProgress;
        private SceneGroupManager m_SceneGroupManager;

        [SerializeField] private LoadingScene defaultLoadingScreen;
        [SerializeField] private LoadingScene gameLoadingScreen;
        [SerializeField] private SceneGroupAsset sceneGroupAssets;
        [SerializeField] private SceneGroupToLoad sceneGroupToLoad;

        private LoadingScene CurrentLoadingScreen { get; set; }

        private const float k_TargetProgress = 1f;
        public bool IsLoading { get; private set; }

        private void Awake()
        {
            CurrentLoadingScreen = defaultLoadingScreen;
            
            ServiceLocator.Global.Register(this, ServiceLevel.Global);

            m_LoadingProgress = new LoadingProgress();
            m_SceneGroupManager = new SceneGroupManager();

            m_SceneGroupManager.OnSceneLoaded +=
                sceneName => Debug.Log("SceneGroupManager OnSceneLoaded: " + sceneName);

            m_SceneGroupManager.OnSceneUnloaded +=
                sceneName => Debug.Log("SceneGroupManager OnSceneUnloaded: " + sceneName);

            m_SceneGroupManager.OnSceneGroupLoaded += FinishedLoading;
        }

        private async void Start()
        {
            m_LoadingProgress.ProgressChanged += ProgressReport;
            await LoadSceneGroupByEnum(sceneGroupToLoad);
        }

        private void Update()
        {
            if (!IsLoading) return;

            UpdateLoadingProgress(CurrentLoadingScreen);
        }

        private async Task LoadSceneGroupByIndex(int index)
        {
            ResetProgressSlider(CurrentLoadingScreen);

            Debug.Log(
                "SceneLoader: ".Color("red") + $"Loading scene group {(SceneGroupToLoad)index}".Color("lightblue"));

            if (index < 0 || index >= sceneGroupAssets.sceneGroups.Count)
            {
                Debug.LogError($"Invalid scene group index: {index}");
            }

            EnableLoadingCanvas(true);
            await m_SceneGroupManager.LoadScenes(sceneGroupAssets.sceneGroups[index], m_LoadingProgress);
        }

        public async Task LoadSceneGroupByEnum(SceneGroupToLoad sceneGroup)
        {
            ResetProgressSlider(CurrentLoadingScreen);

            Debug.Log("SceneLoader: ".Color("red") + $"Loading scene group {sceneGroup.ToString()}".Color("red"));
            EnableLoadingCanvas(true);
            await m_SceneGroupManager.LoadScenes(sceneGroupAssets.sceneGroups[(int)sceneGroup], m_LoadingProgress);
        }

        public async Task LoadSceneGroupByName(string groupName)
        {
            ResetProgressSlider(CurrentLoadingScreen);

            foreach (var sceneGroup in sceneGroupAssets.sceneGroups.Where(sceneGroup =>
                         groupName.Equals(sceneGroup.groupName)))
            {
                EnableLoadingCanvas(true);
                await m_SceneGroupManager.LoadScenes(sceneGroup, m_LoadingProgress);
            }
        }
        
        private void UpdateLoadingProgress(LoadingScene loadingScene)
        {
            var currentFillAmount = loadingScene.progressSlider.value;
            
            loadingScene.progressSlider.value = Mathf.Lerp(currentFillAmount, k_TargetProgress,
                Time.deltaTime * loadingScene.fillSpeed);
            
            loadingScene.loadingText.text = $"{(int)(loadingScene.progressSlider.value * 100)}%";
        }

        private void ResetProgressSlider(LoadingScene loadingScene)
        {
            loadingScene.progressSlider.value = 0f;
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
            IsLoading = enable;
            CurrentLoadingScreen.loadingScreen.SetActive(enable);
        }

        public void SwitchLoadingScreen(LoadingScreenEnum loadingScreenEnum)
        {
            CurrentLoadingScreen = loadingScreenEnum switch
            {
                LoadingScreenEnum.Default => defaultLoadingScreen,
                LoadingScreenEnum.Game => gameLoadingScreen,
                _ => throw new ArgumentOutOfRangeException(nameof(loadingScreenEnum), loadingScreenEnum, null)
            };
        }

        public void SetLoadingScreenTitle(string title)
        {
            CurrentLoadingScreen.loadingText.text = title;
        }
    }
}
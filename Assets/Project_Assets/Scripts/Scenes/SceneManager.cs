using System.Linq;
using System.Threading.Tasks;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework_TempName.ExtensionScripts;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using Project_Assets.Scripts.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Scenes
{
    
    public class SceneManager : MonoBehaviour
    {
        private LoadingProgress m_LoadingProgress;
        private SceneGroupManager m_SceneGroupManager;

        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private SceneGroupAsset sceneGroupAssets;
        [SerializeField] private float fillSpeed;
        [SerializeField] private SceneGroupToLoad sceneGroupToLoad;
        public TMP_Text loadingText;
        public TMP_Text loadingTitleText;

        private const float k_TargetProgress = 1f;
        public bool IsLoading { get; private set; }

        private void Awake()
        {
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
            await LoadSceneGroupByIndex((int)sceneGroupToLoad);
        }

        private void Update()
        {
            if (!IsLoading) return;

            var currentFillAmount = progressSlider.value;
            progressSlider.value = Mathf.Lerp(currentFillAmount, k_TargetProgress, Time.deltaTime * fillSpeed);
        }
        
        private async Task LoadSceneGroupByIndex(int index)
        {
            progressSlider.value = 0f;

            Debug.Log("SceneLoader: ".Color("red") + $"Loading scene group {(SceneGroupToLoad)index}".Color("lightblue"));

            if (index < 0 || index >= sceneGroupAssets.sceneGroups.Count)
            {
                Debug.LogError($"Invalid scene group index: {index}");
            }
            
            EnableLoadingCanvas();
            await m_SceneGroupManager.LoadScenes(sceneGroupAssets.sceneGroups[index], m_LoadingProgress);
        }
        
        public async Task LoadSceneGroupByEnum(SceneGroupToLoad sceneGroup)
        {
            progressSlider.value = 0f;
            
            Debug.Log("SceneLoader: ".Color("red") + $"Loading scene group {sceneGroup.ToString()}".Color("lightblue"));
            EnableLoadingCanvas();
            await m_SceneGroupManager.LoadScenes(sceneGroupAssets.sceneGroups[(int)sceneGroup], m_LoadingProgress);
        }

        public async Task LoadSceneGroupByName(string groupName)
        {
            progressSlider.value = 0f;

            foreach (var sceneGroup in sceneGroupAssets.sceneGroups.Where(sceneGroup => groupName.Equals(sceneGroup.groupName)))
            {
                EnableLoadingCanvas();
                await m_SceneGroupManager.LoadScenes(sceneGroup, m_LoadingProgress);
            }
        }
        
        private void ProgressReport(float value)
        {
        }

        private void FinishedLoading()
        {
            EnableLoadingCanvas(false);
            Debug.Log("SceneLoader: ".Color("red") + "Finished Loading Scene Group".Color("lightblue"));
        }

        private void EnableLoadingCanvas(bool enable = true)
        {
            IsLoading = enable;
            loadingScreen.SetActive(enable);
        }
    }
}
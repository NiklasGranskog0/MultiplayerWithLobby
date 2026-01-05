using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Structs;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project_Assets.Scripts.Scenes
{
    public class SceneGroupManager
    {
        public event Action<string> OnSceneLoaded;
        public event Action<string> OnSceneUnloaded;
        public event Action OnSceneGroupLoaded;

        private SceneGroup m_activeSceneGroup;

        private readonly AsyncOperationGroup m_asyncOperationGroup = new(10);

        public async Task LoadScenes(SceneGroup sceneGroup, IProgress<float> loadingProgress)
        {
            m_activeSceneGroup = sceneGroup;
            var loadedScenes = new List<string>();

            await UnloadScenes();

            var sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;

            for (int i = 0; i < sceneCount; i++)
            {
                loadedScenes.Add(UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).name);
            }

            var totalScenesToLoad = m_activeSceneGroup!.Scenes.Count;

            for (int i = 0; i < totalScenesToLoad; i++)
            {
                var sceneData = sceneGroup!.Scenes[i];

                if (loadedScenes.Contains(sceneData.Name))
                    continue;

                if (sceneData.SceneReference.State == SceneReferenceState.Regular)
                {
                    var asyncOperation =
                        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneData.SceneReference.Path,
                            LoadSceneMode.Additive);

                    m_asyncOperationGroup.AsyncOperations.Add(asyncOperation);
                }

                OnSceneLoaded?.Invoke(sceneData.Name);
            }

            while (!m_asyncOperationGroup.IsDone)
            {
                loadingProgress.Report(m_asyncOperationGroup.Progress);
                await Task.Delay(100);
            }

            var activeScene =
                UnityEngine.SceneManagement.SceneManager.GetSceneByName(
                    m_activeSceneGroup.FindSceneByType(SceneType.ActiveScene));

            if (activeScene.IsValid())
            {
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(activeScene);
            }

            OnSceneGroupLoaded?.Invoke();
        }

        private async Task UnloadScenes(bool unloadUnusedAssets = false)
        {
            var scenes = new HashSet<string>();
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            var sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;

            for (int i = sceneCount - 1; i > 0; i--)
            {
                var sceneAt = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (!sceneAt.isLoaded) continue;

                var sceneName = sceneAt.name;
                scenes.Add(sceneName);
            }

            var asyncOperationGroup = new AsyncOperationGroup(scenes.Count);

            foreach (var scene in scenes)
            {
                if (scene.Equals("StartupScene")) continue;

                var asyncOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
                if (asyncOperation is null) continue;

                asyncOperationGroup.AsyncOperations.Add(asyncOperation);
                OnSceneUnloaded?.Invoke(scene);
            }

            while (!asyncOperationGroup.IsDone)
            {
                await Task.Delay(100);
            }

            if (unloadUnusedAssets)
            {
                await Resources.UnloadUnusedAssets();
            }
        }
    }
}
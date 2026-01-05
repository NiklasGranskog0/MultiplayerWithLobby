using System;
using System.Collections.Generic;
using System.Linq;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Structs;

namespace Project_Assets.Scripts.Scenes
{
    [Serializable]
    public class SceneGroup
    {
        public string GroupName = "Scene Group Name";
        public List<SceneData> Scenes;

        public string FindSceneByType(SceneType sceneType)
        {
            return Scenes.FirstOrDefault(scene => scene.SceneType == sceneType).Name;
        }
    }
}
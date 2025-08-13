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
        public string groupName = "New Scene Group";
        public List<SceneData> scenes;

        public string FindSceneByType(SceneType sceneType)
        {
            return scenes.FirstOrDefault(scene => scene.sceneType == sceneType).Name;
        }
    }

}
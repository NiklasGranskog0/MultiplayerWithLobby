using System;
using Eflatun.SceneReference;
using Project_Assets.Scripts.Enums;

namespace Project_Assets.Scripts.Structs
{
    [Serializable]
    public struct SceneData
    {
        public SceneReference SceneReference;
        public string Name => SceneReference.Name;
        public SceneType SceneType;
    }
}
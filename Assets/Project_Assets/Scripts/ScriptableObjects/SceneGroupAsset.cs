using System.Collections.Generic;
using Project_Assets.Scripts.Scenes;
using UnityEngine;

namespace Project_Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SceneGroupAsset", menuName = "Scriptable Objects/Scene Group Asset")]
    public class SceneGroupAsset : ScriptableObject
    {
        public List<SceneGroup> SceneGroups;
    }
}

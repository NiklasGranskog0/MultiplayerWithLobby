using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using UnityEngine;

namespace Project_Assets.Scripts.Scenes
{
    public class SceneManager : MonoBehaviour
    {
        private void Awake() => ServiceLocator.Global.Register(this, ServiceLevel.Global);
    }
}

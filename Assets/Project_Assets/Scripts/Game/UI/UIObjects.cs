using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Assets.Scripts.Game.UI
{
    public class UIObjects : MonoBehaviour
    {
        public TextMeshProUGUI SelectedObjectName;
        public RawImage SelectedObjectRawImage;
        
        private void Awake() => ServiceLocator.Global.Register(this, ServiceLevel.Global, gameObject.scene.name);
    }
}
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework;
using Project_Assets.Scripts.Framework.ExtensionScripts;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Project_Assets.Scripts.Game.MenuButtons
{
    public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Button Components")]
        public Button ButtonComponent;
        public Image ImageComponent;
        public GameObject ButtonObject;
        
        [HideInInspector] public KeyCode ShortcutKey;
        [HideInInspector] public string TextToolTip = "";
        [HideInInspector] public bool HasToolTip = true;
        
        [Header("Tooltip Components")]
        // Text to set in tooltip if hover over button
        public GameObject TextAreaObject;
        public TextMeshProUGUI TextArea;
        
        [HideInInspector] public SerializedCallback<UnitType> Callback;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!HasToolTip) return;
            
            TextArea.text = TextToolTip;
            TextAreaObject.SetActive(true);
        }
        
        public void OnPointerExit(PointerEventData eventData) => TextAreaObject.SetActive(false);
        public void OnPointerClick(PointerEventData eventData) => Callback?.Invoke();
    }
}

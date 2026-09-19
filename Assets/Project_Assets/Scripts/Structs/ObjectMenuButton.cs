using System;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.UtilityExtensions.Serialization;
using UnityEngine;
using UnityEngine.Events;

namespace Project_Assets.Scripts.Structs
{
    [Serializable]
    public struct ObjectMenuButton
    {
        public UnityEvent ClickedAction;
        public GameMenuButton GameMenuButton;
        public Sprite Icon;
        public KeyCode ShortcutKey;
        [TextArea(5, 1)] public string TextToolTip;

        [Header("Serialized Callback function")]
        [Tooltip("Serialized Callback function")]
        public SerializedCallback<UnitType> Callback;
    }
}
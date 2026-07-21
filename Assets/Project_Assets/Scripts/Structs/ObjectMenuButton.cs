using System;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework;
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

        [Header("Only select this if the button should spawn a unit, otherwise leave it as none")]
        [Tooltip("Only select this if the button should spawn a unit, otherwise leave it as none")]
        public SerializedCallback<UnitType> Callback;
    }
}
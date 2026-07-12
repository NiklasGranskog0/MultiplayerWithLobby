using System;
using Project_Assets.Scripts.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace Project_Assets.Scripts.Structs
{
    [Serializable]
    public struct ObjectMenuButton
    {
        public UnityEvent ClickEvent;
        public GameMenuButton GameMenuButton;
        public Sprite Icon;
        public KeyCode ShortcutKey;
        [TextArea(5, 1)] public string TextToolTip;
    }
}
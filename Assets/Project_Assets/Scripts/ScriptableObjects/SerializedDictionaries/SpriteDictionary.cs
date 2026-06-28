using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Assets.Scripts.ScriptableObjects.SerializedDictionaries
{
    [CreateAssetMenu(fileName = "SpriteDictionary", menuName = "Scriptable Objects/SpriteDictionary")]
    public class SpriteDictionary : ScriptableObject, ISerializationCallbackReceiver
    {
        [Serializable]
        public struct NameAndSprite
        {
            public string Name;
            public Sprite Sprite;
        }

        [SerializeField] private NameAndSprite[] m_sprites;
        [NonSerialized] private Dictionary<string, Sprite> m_dictionary;

        public Sprite this[string spriteName]
        {
            get
            {
                EnsureDictionary();
                return m_dictionary[spriteName];
            }
        }

        private void EnsureDictionary()
        {
            if (m_dictionary == null)
            {
                RebuildDictionary();
            }
        }

        private void RebuildDictionary()
        {
            m_dictionary = new Dictionary<string, Sprite>();

            if (m_sprites == null) return;

            foreach (var nameAndSprite in m_sprites)
            {
                if (string.IsNullOrWhiteSpace(nameAndSprite.Name)) continue;
                m_dictionary[nameAndSprite.Name] = nameAndSprite.Sprite;
            }
        }

        private void OnEnable() => EnsureDictionary();
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() => RebuildDictionary();

#if UNITY_EDITOR
        private void OnValidate() => RebuildDictionary();
#endif
    }
}
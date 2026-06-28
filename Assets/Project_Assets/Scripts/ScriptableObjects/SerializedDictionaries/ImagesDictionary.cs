using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Assets.Scripts.ScriptableObjects.SerializedDictionaries
{
    [CreateAssetMenu(fileName = "Images", menuName = "Scriptable Objects/Images")]
    public class ImagesDictionary : ScriptableObject, ISerializationCallbackReceiver
    {
        [Serializable]
        public struct NameAndTexture
        {
            public string Name;
            public Texture2D Texture;
        }

        [SerializeField] private NameAndTexture[] nameAndTextures;
        [NonSerialized] private Dictionary<string, Texture2D> m_dictionary;

        public Texture2D this[string textureName]
        {
            get
            {
                EnsureDictionary();
                return m_dictionary[textureName];
            }
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            RebuildDictionary();
        }

        private void OnEnable()
        {
            EnsureDictionary();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            RebuildDictionary();
        }
#endif

        private void EnsureDictionary()
        {
            if (m_dictionary == null)
            {
                RebuildDictionary();
            }
        }

        private void RebuildDictionary()
        {
            m_dictionary = new Dictionary<string, Texture2D>();

            if (nameAndTextures == null)
            {
                return;
            }

            foreach (var nameAndTexture in nameAndTextures)
            {
                if (string.IsNullOrWhiteSpace(nameAndTexture.Name))
                {
                    continue;
                }

                m_dictionary[nameAndTexture.Name] = nameAndTexture.Texture;
            }
        }
    }
}

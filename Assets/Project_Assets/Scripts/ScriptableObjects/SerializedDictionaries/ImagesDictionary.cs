using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Assets.Scripts.ScriptableObjects.SerializedDictionaries
{
    [CreateAssetMenu(fileName = "Images", menuName = "Scriptable Objects/Images")]
    public class ImagesDictionary : ScriptableObject
    {
        [Serializable]
        public struct NameAndTexture
        {
            public string Name;
            public Texture2D Texture;
        }
        
        public NameAndTexture[] NameAndTextures;
        private readonly Dictionary<string, Texture2D> m_dictionary = new();

        public Texture2D this[string textureName]
        {
            get
            {
                Init();
                return m_dictionary[textureName];
            }
            
        }

        private void Init()
        {
            foreach (var nameAndTexture in NameAndTextures)
            {
                m_dictionary[nameAndTexture.Name] = nameAndTexture.Texture;
            }
        }
    }
}

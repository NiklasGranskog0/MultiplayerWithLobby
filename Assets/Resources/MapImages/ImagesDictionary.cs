using System;
using System.Collections.Generic;
using UnityEngine;

namespace Resources.MapImages
{
    [CreateAssetMenu(fileName = "Images", menuName = "Scriptable Objects/Images")]
    public class ImagesDictionary : ScriptableObject
    {
        [Serializable]
        public struct NameAndTexture
        {
            public string name;
            public Texture2D texture;
        }
        
        public NameAndTexture[] nameAndTextures;
        private readonly Dictionary<string, Texture2D> m_Dictionary = new();

        public Texture2D this[string textureName]
        {
            get
            {
                Init();
                return m_Dictionary[textureName];
            }
            
        }

        private void Init()
        {
            foreach (var nameAndTexture in nameAndTextures)
            {
                m_Dictionary[nameAndTexture.name] = nameAndTexture.texture;
            }
        }
    }
}

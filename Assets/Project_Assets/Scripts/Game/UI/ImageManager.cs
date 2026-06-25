using System;
using System.Collections.Generic;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework_TempName.UnityServiceLocator;
using UnityEngine;

namespace Project_Assets.Scripts.Game.UI
{
    public class ImageManager : MonoBehaviour
    {
        // TODO: Add the animation component, so that the image can show the current animation running instead of only idle
        public Camera ImageCamera;
        private ImageToLoad m_currentImage;
        
        [Serializable]
        public struct FromEnumToGameObject
        {
            public ImageToLoad ImageToLoad;
            public GameObject GameObject;
        }
        
        [SerializeField] private FromEnumToGameObject[] m_fromEnumToGameObject;
        private readonly Dictionary<ImageToLoad, GameObject> m_imageToGameObject = new();

        private void Awake()
        {
            ServiceLocator.Global.Register(this, ServiceLevel.Scene, gameObject.scene.name);

            foreach (var imageEnum in m_fromEnumToGameObject)
            {
                m_imageToGameObject[imageEnum.ImageToLoad] = imageEnum.GameObject;
            }
        } 

        public void LoadImage(ImageToLoad image)
        {
            m_currentImage = image;
            m_imageToGameObject[m_currentImage].SetActive(true);
        }

        public void UnloadImage()
        {
            m_imageToGameObject[m_currentImage].SetActive(false);
        }
    }
}
using System;
using System.Collections.Generic;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Framework.UnityServiceLocator;
using UnityEngine;

namespace Project_Assets.Scripts.Game.UI
{
    public class ImageManager : MonoBehaviour
    {
        public Camera ImageCamera;
        private ImageToLoad m_currentImage;
        
        [Serializable]
        public struct FromEnumToGameObject
        {
            public ImageToLoad ImageToLoad;
            public GameObject GameObject;
        }
        
        // Populate this array in the inspector with the game objects corresponding to enums
        [SerializeField] private FromEnumToGameObject[] m_fromEnumToGameObject;
        private readonly Dictionary<ImageToLoad, GameObject> m_imageToGameObject = new();

        private void Awake()
        {
            ServiceLocator.Global.Register(this, ServiceLevel.Scene, gameObject.scene.name);

            // Sets the enum key to the corresponding game object from the array m_fromEnumToGameObject
            foreach (var imageEnum in m_fromEnumToGameObject)
            {
                m_imageToGameObject[imageEnum.ImageToLoad] = imageEnum.GameObject;
            }
        } 

        // Sets the game object corresponding to the image to active
        public void LoadImage(ImageToLoad image)
        {
            m_imageToGameObject[m_currentImage].SetActive(false);
            if (image == ImageToLoad.None) return;
            
            m_currentImage = image;
            m_imageToGameObject[m_currentImage].SetActive(true);
        }
    }
}
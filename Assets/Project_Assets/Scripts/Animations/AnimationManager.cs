using System;
using System.Collections;
using UnityEngine;

namespace Project_Assets.Scripts.Animations
{
    [Serializable]
    public class AnimationManager<T> where T : Enum
    {
        public AnimationData data;
        public Animator animator;

        public bool[] layerLocked;
        public T[] currentAnimation;

        public T GetCurrentAnimation(int layer) => currentAnimation[layer];

        public void Initialize(T startAnimation)
        {
            var layers = animator.layerCount;
            layerLocked = new bool[layers];
            currentAnimation = new T[layers];

            for (int i = 0; i < layers; i++)
            {
                layerLocked[i] = false;
                currentAnimation[i] = startAnimation;
            }
        }

        public void SetLocked(bool lockLayer, int layer)
        {
            layerLocked[layer] = lockLayer;
        }

        public void Play(T animationEnum, int layer, bool lockLayer = false, bool overrideLock = false,
            float crossFade = 0.2f)
        {
            if (layerLocked[layer] && !overrideLock) return;

            layerLocked[layer] = lockLayer;

            if (currentAnimation[layer].Equals(animationEnum)) return;

            currentAnimation[layer] = animationEnum;

            var stateHash = data.EnumToStateHash(animationEnum);
            animator.CrossFade(stateHash, crossFade, layer);
        }

        public void DelayedPlay(MonoBehaviour coroutineStarter, T animationEnum, float delay, int layer,
            bool lockLayer = false, bool overrideLock = false, float crossFade = 0.2f)
        {
            if (delay > 0f)
            {
                coroutineStarter.StartCoroutine(Delay());

                IEnumerator Delay()
                {
                    yield return new WaitForSeconds(delay - crossFade);
                    Play(animationEnum, layer, lockLayer, overrideLock, crossFade);
                }
            }
        }
    }
}
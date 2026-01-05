using System;
using System.Collections;
using UnityEngine;

namespace Project_Assets.Scripts.Animations
{
    [Serializable]
    public class AnimationManager<T> where T : Enum
    {
        public AnimationData Data;
        public Animator Animator;

        public bool[] LayerLocked;
        public T[] CurrentAnimation;

        public T GetCurrentAnimation(int layer) => CurrentAnimation[layer];

        public void Initialize(T startAnimation)
        {
            var layers = Animator.layerCount;
            LayerLocked = new bool[layers];
            CurrentAnimation = new T[layers];

            for (int i = 0; i < layers; i++)
            {
                LayerLocked[i] = false;
                CurrentAnimation[i] = startAnimation;
            }
        }

        public void SetLocked(bool lockLayer, int layer)
        {
            LayerLocked[layer] = lockLayer;
        }

        public void Play(T animationEnum, int layer, bool lockLayer = false, bool overrideLock = false,
            float crossFade = 0.2f)
        {
            if (LayerLocked[layer] && !overrideLock) return;

            LayerLocked[layer] = lockLayer;

            if (CurrentAnimation[layer].Equals(animationEnum)) return;

            CurrentAnimation[layer] = animationEnum;

            var stateHash = Data.EnumToStateHash(animationEnum);
            Animator.CrossFade(stateHash, crossFade, layer);
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
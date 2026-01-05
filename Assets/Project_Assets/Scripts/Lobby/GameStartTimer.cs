using System;
using System.Collections;
using UnityEngine;

namespace Project_Assets.Scripts.Lobby
{
    public class GameStartTimer : MonoBehaviour
    {
        private const float k_timer = 5f;
        private float TimerLeft { get; set; }

        public event Action<float> OnTimerLeft;
        public event Action OnTimerFinished;

        public void StartTimer()
        {
            TimerLeft = k_timer;
            StartCoroutine(TimerCoroutine());
        }

        private IEnumerator TimerCoroutine()
        {
            while (TimerLeft > 0f)
            {
                yield return new WaitForSeconds(1);
                OnTimerLeft?.Invoke(TimerLeft);
                TimerLeft -= 1f;

                if (!(TimerLeft <= 0f)) continue;
                
                yield return new WaitForSeconds(1f);
                OnTimerFinished?.Invoke();
            }
        }
    }
}

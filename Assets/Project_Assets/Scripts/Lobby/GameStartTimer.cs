using System;
using System.Collections;
using UnityEngine;

namespace Project_Assets.Scripts.Lobby
{
    public class GameStartTimer : MonoBehaviour
    {
        private const float k_Timer = 5f;
        public float TimerLeft { get; set; }

        public event Action<float> OnTimerLeft;
        public event Action OnTimerFinished;

        public void StartTimer()
        {
            TimerLeft = k_Timer;
            StartCoroutine(TimerCoroutine());
        }

        private IEnumerator TimerCoroutine()
        {
            while (TimerLeft >= 0)
            {
                yield return new WaitForSeconds(1);
                OnTimerLeft?.Invoke(TimerLeft);
                TimerLeft -= 1;

                if (!(TimerLeft <= 0)) continue;
                
                yield return new WaitForSeconds(1);
                OnTimerFinished?.Invoke();
            }
        }
    }
}

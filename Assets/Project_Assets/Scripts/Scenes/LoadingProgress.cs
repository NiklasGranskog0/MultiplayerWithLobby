using System;

namespace Project_Assets.Scripts.Scenes
{
    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> OnProgressChanged;
        private const float k_ratio = 0.9f;
        public void Report(float value)
        {
            OnProgressChanged?.Invoke(value / k_ratio);
        }
    }
}
using System;

namespace Project_Assets.Scripts.Scenes
{
    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> ProgressChanged;
        private const float k_Ratio = 0.9f;
        public void Report(float value)
        {
            ProgressChanged?.Invoke(value / k_Ratio);
        }
    }
}
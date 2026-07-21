using Project_Assets.Scripts.Framework.ExtensionScripts;
using UnityEngine;

namespace Project_Assets.Scripts.Framework
{
    public class SerializedTest : MonoBehaviour
    {
        [SerializeField] private SerializedCallback<int> m_callback;

        private void Start()
        {
            var result = m_callback?.Invoke();
            Debug.Log($"Callback result: {result}".Color(Color.lightSalmon));
        }

        public int MultiplyByTwo(int value)
        {
            return value * 2;
        }
        
        public int AddNumbersTogether(int a, int b)
        {
            return a + b;
        }

        public int MultiplyMagnitude(int factor, Vector3 vector)
        {
            return (int)(vector.magnitude * factor);
        }
    }
}
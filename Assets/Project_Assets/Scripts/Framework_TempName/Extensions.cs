using UnityEngine;

namespace Project_Assets.Scripts.Framework_TempName
{
    public static class Extensions
    {
        public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;

        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.TryGetComponent<T>(out var component) ? component : gameObject.AddComponent<T>();
        }
        
        public static string Color(this string s, string color) => $"<color={color.ToUpper()}>{s}</color>";
    }
}

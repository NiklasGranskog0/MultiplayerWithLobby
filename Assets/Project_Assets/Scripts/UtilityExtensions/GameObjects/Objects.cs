using System;
using UnityEngine;

namespace Project_Assets.Scripts.UtilityExtensions.GameObjects
{
    public static class Objects
    {
        public static T OrNull<T>(this T obj) where T : UnityEngine.Object => obj ? obj : null;

        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.TryGetComponent<T>(out var component) ? component : gameObject.AddComponent<T>();
        }

        public static T Get<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.TryGetComponent(out T component)
                ? component
                : throw new ArgumentException(
                    $"GameObject {gameObject.name} does not have a component of type {typeof(T).Name}");
        }
    }
}
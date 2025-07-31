using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Assets.Scripts.Framework_TempName.UnityServiceLocator
{
    public class ServiceManager
    {
        private readonly Dictionary<Type, object> m_Services = new();
        public IEnumerable<object> RegisteredServices => m_Services.Values;

        public bool TryGet<T>(out T service) where T : class
        {
            var type = typeof(T);
            if (m_Services.TryGetValue(type, out object obj))
            {
                service = obj as T;
                return true;
            }

            service = null;
            return false;
        }
        
        public T Get<T>() where T : class
        {
            var type = typeof(T);
            if (m_Services.TryGetValue(type, out object obj)) return obj as T;
            
            throw new ArgumentException($"ServiceManager.Get<T>(): Service of type {type.FullName} not registered.");
        }
        
        public ServiceManager Register<T>(T service)
        {
            var type = typeof(T);
            if (!m_Services.TryAdd(type, service)) Debug.LogError($"Service {type} already registered");
            
            return this;
        }

        public ServiceManager Register(Type type, object service)
        {
            if (!type.IsInstanceOfType(service)) throw new ArgumentException($"Service {type} is not of type {type}");

            if (!m_Services.TryAdd(type, service)) Debug.LogError($"Service {type} already registered");
            
            return this;
        }
    }
}

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Project_Assets.Scripts.Structs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project_Assets.Scripts.Framework
{
    [Serializable]
    public class SerializedCallback<TReturn> : ISerializationCallbackReceiver
    {
        [SerializeField] private Object m_targetObject;
        [SerializeField] private string m_methodName;
        [SerializeField] private AnyValue[] m_parameters;
        
        [NonSerialized] private Delegate m_cachedDelegate;
        [NonSerialized] private bool m_isDelegateRebuilt;

        public TReturn Invoke() => Invoke(m_parameters);

        public TReturn Invoke(params AnyValue[] parameters)
        {
            if (!m_isDelegateRebuilt) BuildDelegate();

            if (m_cachedDelegate != null)
            {
                var result = m_cachedDelegate.DynamicInvoke(ConvertParameters(parameters));
                return (TReturn)Convert.ChangeType(result, typeof(TReturn));
            }
            
            Debug.LogWarning($"Unable to invoke method {m_methodName} on {m_targetObject} with parameters {parameters}");
            return default;
        }

        private object[] ConvertParameters(AnyValue[] args)
        {
            if (args == null || args.Length == 0) return Array.Empty<object>();

            var convertedParams = new object[args.Length];
            
            for (int i = 0; i < args.Length; i++)
            {
                convertedParams[i] = args[i].ConvertValue<object>();
            }
            
            return convertedParams;
        }

        private void BuildDelegate()
        {
            m_cachedDelegate = null;

            if (m_targetObject == null || string.IsNullOrEmpty(m_methodName))
            {
                Debug.LogWarning("Target object or method name is null, cannot rebuild delegate");
                return;
            }
            
            Type targetType = m_targetObject.GetType();
            MethodInfo methodInfo = targetType.GetMethod(m_methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (methodInfo == null)
            {
                Debug.LogWarning($"Method {m_methodName} not found on {m_targetObject}");
                return;
            }

            Type[] parameterTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
            
            if (m_parameters.Length != parameterTypes.Length)
            {
                Debug.LogWarning($"Parameter mismatch for method {m_methodName}");
                return;
            }

            Type delegateType = Expression.GetDelegateType(parameterTypes.Append(methodInfo.ReturnType).ToArray());
            m_cachedDelegate = methodInfo.CreateDelegate(delegateType, m_targetObject);
            m_isDelegateRebuilt = true;
        }
        
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() => m_isDelegateRebuilt = false;
    }
}
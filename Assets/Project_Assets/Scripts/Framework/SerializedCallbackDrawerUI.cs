using System;
using System.Linq;
using System.Reflection;
using Project_Assets.Scripts.Enums;
using Project_Assets.Scripts.Structs;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using ValueType = Project_Assets.Scripts.Enums.ValueType;

namespace Project_Assets.Scripts.Framework
{
    [CustomPropertyDrawer(typeof(SerializedCallback<>), true)]
    public class SerializedCallbackDrawerUI : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();
            
            SerializedProperty targetProperty = property.FindPropertyRelative("m_targetObject");
            ObjectField targetField = new("Target")
            {
                objectType = typeof(Object),
                bindingPath = targetProperty.propertyPath
            };
            root.Add(targetField);
            
            SerializedProperty methodProperty = property.FindPropertyRelative("m_methodName");
            Button methodField = new()
            {
                text = string.IsNullOrEmpty(methodProperty.stringValue) ? "Select Method" : methodProperty.stringValue,
            };
            root.Add(methodField);

            methodField.clicked += () => ShowMethodDropdown(targetProperty.objectReferenceValue, methodProperty, property, methodField, root);

            SerializedProperty parametersProperty = property.FindPropertyRelative("m_parameters");
            VisualElement parametersContainer = new();
            root.Add(parametersContainer);

            UpdateParameters(parametersProperty, parametersContainer);
            
            property.serializedObject.ApplyModifiedProperties();
            
            return root;
        }

        private void ShowMethodDropdown(Object target, SerializedProperty methodProperty, SerializedProperty property, Button methodButton, VisualElement root)
        {
            if (target == null) return;
            
            GenericMenu menu = new();
            Type targetType = target.GetType();

            Type callbackType = fieldInfo.FieldType;
            Type genericType = callbackType.GetGenericArguments()[0]; // this was to get methods with same valuetype i think

            if (callbackType.IsGenericType)
            {
                var methods = targetType
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.ReturnType == genericType).ToArray();

                foreach (MethodInfo method in methods)
                {
                    menu.AddItem(
                        new GUIContent(method.Name),
                        false,
                        () =>
                        {
                            methodProperty.stringValue = method.Name;
                            methodButton.text = method.Name;
                            
                            SerializedProperty parametersProperty = property.FindPropertyRelative("m_parameters");
                            var parameters = method.GetParameters();
                            parametersProperty.arraySize = parameters.Length;

                            for (int i = 0; i < parameters.Length; i++)
                            {
                                SerializedProperty paramProp = parametersProperty.GetArrayElementAtIndex(i);
                                SerializedProperty typeProp = paramProp.FindPropertyRelative("ValueType");
                                typeProp.enumValueIndex = (int)AnyValue.ValueTypeOf(parameters[i].ParameterType);
                            }
                            
                            property.serializedObject.ApplyModifiedProperties();

                            VisualElement parametersContainer = root.Children().Last();
                            parametersContainer.Clear();
                            UpdateParameters(parametersProperty, parametersContainer);
                        });

                    if (!methods.Any())
                    {
                        menu.AddDisabledItem(new GUIContent("No methods found"));
                    }
                    
                    menu.ShowAsContext();
                }
            }
        }
        
        private void UpdateParameters(SerializedProperty parametersProp, VisualElement container)
        {
            if (!parametersProp.isArray) return;

            for (int i = 0; i < parametersProp.arraySize; i++)
            {
                SerializedProperty parameter = parametersProp.GetArrayElementAtIndex(i);
                SerializedProperty typeProp = parameter.FindPropertyRelative("ValueType");

                ValueType paramType = (ValueType)typeProp.enumValueIndex;
                VisualElement field;

                switch (paramType)
                {
                    case ValueType.Int:
                        SerializedProperty intProp = parameter.FindPropertyRelative("IntValue");
                        IntegerField intField = new($"Parameter {i + 1} (Int)");
                        intField.value = intProp.intValue;
                        intField.RegisterValueChangedCallback(evt =>
                        {
                            intProp.intValue = evt.newValue;
                            parametersProp.serializedObject.ApplyModifiedProperties();
                        });
                        field = intField;
                        break;
                    
                    case ValueType.Float:
                        SerializedProperty floatProp = parameter.FindPropertyRelative("FloatValue");
                        FloatField floatField = new($"Parameter {i + 1} (Float)");
                        floatField.value = floatProp.floatValue;
                        floatField.RegisterValueChangedCallback(evt =>
                        {
                            floatProp.floatValue = evt.newValue;
                            parametersProp.serializedObject.ApplyModifiedProperties();
                        });
                        field = floatField;
                        break;
                    
                    case ValueType.String:
                        SerializedProperty stringProp = parameter.FindPropertyRelative("StringValue");
                        TextField stringField = new($"Parameter {i + 1} (String)");
                        stringField.value = stringProp.stringValue;
                        stringField.RegisterValueChangedCallback(evt =>
                        {
                            stringProp.stringValue = evt.newValue;
                            parametersProp.serializedObject.ApplyModifiedProperties();
                        });
                        field = stringField;
                        break;
                        
                    case ValueType.Bool:
                        SerializedProperty boolProp = parameter.FindPropertyRelative("BoolValue");
                        Toggle boolField = new($"Parameter {i + 1} (Bool)");
                        boolField.value = boolProp.boolValue;
                        boolField.RegisterValueChangedCallback(evt =>
                        {
                            boolProp.boolValue = evt.newValue;
                            parametersProp.serializedObject.ApplyModifiedProperties();
                        });
                        field = boolField;
                        break;
                    
                    case ValueType.Vector3:
                        SerializedProperty vector3Prop = parameter.FindPropertyRelative("Vector3Value");
                        Vector3Field vector3Field = new($"Parameter {i + 1} (Vector3)");
                        vector3Field.value = vector3Prop.vector3Value;
                        vector3Field.RegisterValueChangedCallback(evt =>
                        {
                            vector3Prop.vector3Value = evt.newValue;
                            parametersProp.serializedObject.ApplyModifiedProperties();
                        });
                        field = vector3Field;
                        break;
                    
                    case ValueType.UnitTypeEnum:
                        SerializedProperty enumProp = parameter.FindPropertyRelative("UnitTypeValue");
                        EnumField enumField = new($"Parameter {i + 1} (UnitType)", enumProp.GetEnumValue<UnitType>());
                        enumField.RegisterValueChangedCallback(evt =>
                        {
                            enumProp.enumValueIndex = GetEnumIndex(evt.newValue, typeof(UnitType));
                            parametersProp.serializedObject.ApplyModifiedProperties();
                        });
                        field = enumField;
                        break;

                    case ValueType.GameMenuButtonEnum:
                        SerializedProperty gameMenuButtonProp = parameter.FindPropertyRelative("GameMenuButtonValue");
                        EnumField gameMenuButtonField = new($"Parameter {i + 1} (GameMenuButton)", gameMenuButtonProp.GetEnumValue<GameMenuButton>());
                        gameMenuButtonField.RegisterValueChangedCallback(evt =>
                        {
                            gameMenuButtonProp.enumValueIndex = GetEnumIndex(evt.newValue, typeof(GameMenuButton));
                            parametersProp.serializedObject.ApplyModifiedProperties();
                        });
                        field = gameMenuButtonField;
                        break;
                    
                    default:
                        field = new Label($"Parameter {i + 1} (Unsupported Type)");
                        break;
                }
                
                container.Add(field);
            }
        }

        private static int GetEnumIndex(Enum value, Type enumType)
        {
            Array values = Enum.GetValues(enumType);

            for (int i = 0; i < values.Length; i++)
            {
                if (Equals(values.GetValue(i), value)) return i;
            }

            return 0;
        }
    }
}

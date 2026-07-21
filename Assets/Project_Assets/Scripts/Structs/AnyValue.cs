using System;
using UnityEngine;

namespace Project_Assets.Scripts.Structs
{
    [Serializable]
    public struct AnyValue
    {
        public Enums.ValueType ValueType;
        
        public bool BoolValue;
        public int IntValue;
        public float FloatValue;
        public string StringValue;
        public Vector3 Vector3Value;
        public Enums.UnitType UnitTypeValue;
        public Enums.GameMenuButton GameMenuButtonValue;
        
        public static implicit operator bool(AnyValue value) => value.ConvertValue<bool>();
        public static implicit operator int(AnyValue value) => value.ConvertValue<int>();
        public static implicit operator float(AnyValue value) => value.ConvertValue<float>();
        public static implicit operator string(AnyValue value) => value.ConvertValue<string>();
        public static implicit operator Vector3(AnyValue value) => value.ConvertValue<Vector3>();
        public static implicit operator Enums.UnitType(AnyValue value) => value.ConvertValue<Enums.UnitType>();
        public static implicit operator Enums.GameMenuButton(AnyValue value) => value.ConvertValue<Enums.GameMenuButton>();

        public T ConvertValue<T>()
        {
            if (typeof(T) == typeof(object)) return CastToObject<T>();

            return ValueType switch
            {
                Enums.ValueType.Bool => AsBool<T>(BoolValue),
                Enums.ValueType.Int => AsInt<T>(IntValue),
                Enums.ValueType.Float => AsFloat<T>(FloatValue),
                Enums.ValueType.String => (T)(object)StringValue,
                Enums.ValueType.Vector3 => AsVector3<T>(Vector3Value),
                Enums.ValueType.UnitTypeEnum => AsUnitType<T>(UnitTypeValue),
                Enums.ValueType.GameMenuButtonEnum => AsGameMenuButton<T>(GameMenuButtonValue),
                _ => throw new InvalidCastException($"Cannot convert AnyValue of type {ValueType} to {typeof(T).Name}")
            };
        }
        
        private T AsBool<T>(bool value) => typeof(T) == typeof(bool) && value is T correctType ? correctType : default;
        private T AsInt<T>(int value) => typeof(T) == typeof(int) && value is T correctType ? correctType : default;
        private T AsFloat<T>(float value) => typeof(T) == typeof(float) && value is T correctType ? correctType : default;
        private T AsVector3<T>(Vector3 value) => typeof(T) == typeof(Vector3) && value is T correctType ? correctType : default;
        private T AsUnitType<T>(Enums.UnitType value) => typeof(T) == typeof(Enums.UnitType) && value is T correctType ? correctType : default;
        private T AsGameMenuButton<T>(Enums.GameMenuButton value) => typeof(T) == typeof(Enums.GameMenuButton) && value is T correctType ? correctType : default;

        public static Type TypeOf(Enums.ValueType valueType)
        {
            return valueType switch
            {
                Enums.ValueType.Bool => typeof(bool),
                Enums.ValueType.Int => typeof(int),
                Enums.ValueType.Float => typeof(float),
                Enums.ValueType.String => typeof(string),
                Enums.ValueType.Vector3 => typeof(Vector3),
                Enums.ValueType.UnitTypeEnum => typeof(Enums.UnitType),
                Enums.ValueType.GameMenuButtonEnum => typeof(Enums.GameMenuButton),
                _ => throw new NotSupportedException($"Value type not supported: {valueType}")
            };
        }
        
        public static Enums.ValueType ValueTypeOf(Type type)
        {
            return type switch
            {
                _ when type == typeof(bool) => Enums.ValueType.Bool,
                _ when type == typeof(int) => Enums.ValueType.Int,
                _ when type == typeof(float) => Enums.ValueType.Float,
                _ when type == typeof(string) => Enums.ValueType.String,
                _ when type == typeof(Vector3) => Enums.ValueType.Vector3,
                _ when type == typeof(Enums.UnitType) => Enums.ValueType.UnitTypeEnum,
                _ when type == typeof(Enums.GameMenuButton) => Enums.ValueType.GameMenuButtonEnum,
                _ => throw new NotSupportedException($"Type not supported: {type}")
            };
        }
        
        private T CastToObject<T>()
        {
            return ValueType switch
            {
                Enums.ValueType.Int => (T)(object)IntValue,
                Enums.ValueType.Float => (T)(object)FloatValue,
                Enums.ValueType.Bool => (T)(object)BoolValue,
                Enums.ValueType.String => (T)(object)StringValue,
                Enums.ValueType.Vector3 => (T)(object)Vector3Value,
                Enums.ValueType.UnitTypeEnum => (T)(object)UnitTypeValue,
                Enums.ValueType.GameMenuButtonEnum => (T)(object)GameMenuButtonValue,
                _ => throw new InvalidCastException($"Cannot convert AnyValue of type {ValueType} to {typeof(T).Name}")
            };
        }
        
    }
}

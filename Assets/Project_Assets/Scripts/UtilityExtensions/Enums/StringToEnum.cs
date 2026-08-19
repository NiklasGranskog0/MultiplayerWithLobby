using System;
using Project_Assets.Scripts.Enums;

namespace Project_Assets.Scripts.UtilityExtensions.Enums
{
    public static class StringToEnum
    {
        // Used to get unit type from prefab name, which have an underscore after each word
        public static UnitType UnitTypeFromString(this string s)
        {
            var stringType = s.Replace("_", "");

            if (Enum.TryParse<UnitType>(stringType, out var type)) return type;

            throw new ArgumentException($"UnitTypeFromString: Tried to convert {s} to {stringType} Enum," +
                                        "but it does not exist.");
        }
    }
}
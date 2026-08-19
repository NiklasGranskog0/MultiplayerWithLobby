using UnityEngine;

namespace Project_Assets.Scripts.UtilityExtensions.Strings
{
    public static class Colors
    {
        private static byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        private static string ToHex(this Color color) =>
            $"#{ToByte(color.r):X2}{ToByte(color.g):X2}{ToByte(color.b):X2}";

        public static string Color(this string text, Color color) => $"<color={color.ToHex()}>{text}</color>";
    }
}
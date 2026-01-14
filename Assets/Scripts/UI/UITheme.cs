using UnityEngine;

namespace CoinsOfHope.UI
{
    public static class UITheme
    {
        public static readonly Color Primary = Hex("2EC4B6");
        public static readonly Color Secondary = Hex("FF9F1C");
        public static readonly Color Background = Hex("FDFFFC");
        public static readonly Color Text = Hex("2B2D42");
        public static readonly Color MutedText = Hex("6B7280");

        private static Color Hex(string hex)
        {
            if (ColorUtility.TryParseHtmlString($"#{hex}", out var c)) return c;
            return Color.white;
        }
    }
}

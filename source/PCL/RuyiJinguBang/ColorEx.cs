using System;

using Xamarin.Forms;

namespace RuyiJinguBang
{
    public static class ColorEx
    {
        public static Color OrDefault(this Color Self, Color DefaultColor)
        {
            return Color.Default != Self ? Self : DefaultColor;
        }
    }
}

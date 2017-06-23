using System;

using Xamarin.Forms;
using SkiaSharp;

namespace RuyiJinguBang
{
    public static class ColorEx
    {
        public static Color OrDefault(this Color Self, Color DefaultColor)
        {
            return Color.Default != Self ? Self : DefaultColor;
        }
        public static Color MergeWithRate(Color A, Color B, double Rate)
        {
            if (Rate <= 0.0)
            {
                return A;
            }
            else
            if (1.0 <= Rate)
            {
                return B;
            }
            else
            {
                var RateA = 1.0 - Rate;
                var RateB = Rate;
                return new Color
                (
                    (A.R * RateA) + (B.R * RateB),
                    (A.G * RateA) + (B.G * RateB),
                    (A.B * RateA) + (B.B * RateB),
                    (A.A * RateA) + (B.A * RateB)
                );
            }
        }
        public static Color ToColor(this SKColor Skia)
        {
            return new Color
            (
                ((double)(Skia.Red)) /255.0,
                ((double)(Skia.Green)) / 255.0,
                ((double)(Skia.Blue)) / 255.0,
                ((double)(Skia.Alpha)) / 255.0
            );
        }
    }
}

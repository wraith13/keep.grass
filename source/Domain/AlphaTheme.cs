using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RuyiJinguBang;
using SkiaSharp;

namespace keep.grass.Domain
{
    public class AlphaTheme
    {
        public SKColor AccentColor
        {
            get;
            private set;
        }
        public SKColor ForegroundColor
        {
            get;
            private set;
        }
        public SKColor BackgroundColor
        {
            get;
            private set;
        }
        public Func<double, SKColor> MakeLeftTimeColor
        {
            get;
            private set;
        }

        private AlphaTheme()
        {
        }

        public static AlphaTheme AlterWhite = new AlphaTheme // Color.Default を使ってないバージョン
        {
            AccentColor = new SKColor(0x44, 0x55, 0xEE),
            ForegroundColor = new SKColor(0x00, 0x00, 0x00),
            BackgroundColor = new SKColor(0xFF, 0xFF, 0xFF),
            MakeLeftTimeColor = LeftTimeRate => new SKColor
            (
                red: (byte)(255.0 * (1.0 - LeftTimeRate)),
                green: (byte)(255.0 * Math.Min(0.5, LeftTimeRate)),
                blue: 0
            ),
        };
        /*
        public static AlphaTheme DefaultWhite = new AlphaTheme
        {
            AccentColor = Color.Default,// new SKColor(0x44, 0x55, 0xEE),
            ForegroundColor = Color.Default,
            BackgroundColor = Color.Default,
            MakeLeftTimeColor = LeftTimeRate => new SKColor
            (
                red: (byte)(255.0 * (1.0 - LeftTimeRate)),
                green: (byte)(255.0 * Math.Min(0.5, LeftTimeRate)),
                blue: 0
            ),
        };
        */
        public static AlphaTheme White = AlterWhite;
        public static AlphaTheme Grass = new AlphaTheme
        {
            AccentColor = new SKColor(0x00, 0x00, 0x00),
            ForegroundColor = new SKColor(0x10, 0x60, 0x20),
            BackgroundColor = new SKColor(0x88, 0xEE, 0x99),
            MakeLeftTimeColor = LeftTimeRate => new SKColor
            (
                red: (byte)(160.0 * (1.0 - LeftTimeRate)),
                green: (byte)(255.0 * Math.Min(0.5, LeftTimeRate)),
                blue: (byte)(100.0 * (1.0 - LeftTimeRate))
            ),
        };
        public static AlphaTheme Cherry = new AlphaTheme
        {
            AccentColor = new SKColor(0xFF, 0x00, 0x00),
            ForegroundColor = new SKColor(0x80, 0x40, 0x40),
            BackgroundColor = new SKColor(0xFF, 0xCC, 0xCC),
            MakeLeftTimeColor = LeftTimeRate => new SKColor
            (
                red: (byte)(255.0 * (1.0 - LeftTimeRate)),
                green: (byte)(240.0 * Math.Min(0.5, LeftTimeRate)),
                blue: (byte)(100.0 * Math.Min(0.5, LeftTimeRate))
            ),
        };
        public static AlphaTheme Abyss = new AlphaTheme
        {
            AccentColor = new SKColor(0x44, 0x55, 0xEE),
            ForegroundColor = new SKColor(0xAA, 0xBB, 0xEE),
            BackgroundColor = new SKColor(0x11, 0x33, 0x66),
            MakeLeftTimeColor = LeftTimeRate => new SKColor
            (
                red: (byte)(240.0 * (1.0 - LeftTimeRate)),
                green: (byte)(240.0 * Math.Min(0.5, LeftTimeRate)),
                blue: (byte)40
            ),
        };
        public static AlphaTheme Black = new AlphaTheme
        {
            AccentColor = new SKColor(0x44, 0x55, 0xEE),
            ForegroundColor = new SKColor(0xE8, 0xF0, 0xEC),
            BackgroundColor = new SKColor(0x00, 0x00, 0x00),
            MakeLeftTimeColor = LeftTimeRate => new SKColor
            (
                red: (byte)(160.0 * (1.0 - LeftTimeRate)),
                green: (byte)(160.0 * Math.Min(0.5, LeftTimeRate)),
                blue: (byte)40
            ),
        };

        public static Dictionary<string, AlphaTheme> All = new Dictionary<string, AlphaTheme>
        {
            { nameof(White), White },
            { nameof(Grass), Grass },
            { nameof(Cherry), Cherry },
            { nameof(Abyss), Abyss },
            //{ nameof(Black), Black }, 黒背景は問題が発生しやすいので一旦止血
        };

        private static KeyValuePair<string, AlphaTheme> Get(string Theme)
        {
            return All
                .Where(i => i.Key == Theme)
                .Concat(All.FirstOrDefault())
                .FirstOrDefault();
        }

        private static AlphaTheme Cache = null;

        public static AlphaTheme Get()
        {
            return Cache ??
            (
                Cache = Get(Settings.Theme).Value
            );
        }
        public static void Set(String Theme)
        {
            var i = Get(Theme);
            Cache = i.Value;
            Settings.Theme = i.Key;
        }
    }
}

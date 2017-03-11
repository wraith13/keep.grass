using System;
using System.Collections.Generic;
using System.Linq;
using keep.grass.Helpers;
using Xamarin.Forms;

namespace keep.grass
{
    public class AlphaTheme
    {
        public Color AccentColor
        {
            get;
            private set;
        }
        public Color ForeGroundColor
        {
            get;
            private set;
        }
        public Color BackGroundColor
        {
            get;
            private set;
        }
        public bool IsLightTheme
        {
            get;
            private set;
        }

        private AlphaTheme()
        {
        }

        public static AlphaTheme White = new AlphaTheme
        {
            AccentColor = Color.Default,// Color.FromRgb(0x44, 0x55, 0xEE),
            ForeGroundColor = Color.Black,
            BackGroundColor = Color.White,
            IsLightTheme = true,
        };
        public static AlphaTheme Black = new AlphaTheme
        {
            AccentColor = Color.Default,// Color.FromRgb(0x44, 0x55, 0xEE),
            ForeGroundColor = Color.White,
            BackGroundColor = Color.Black,
            IsLightTheme = false,
        };

        public static Dictionary<string, AlphaTheme> All = new Dictionary<string, AlphaTheme>
        {
            { nameof(White), White },
            { nameof(Black), Black },
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

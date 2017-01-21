using System;
using System.Collections.Generic;
using System.Linq;
using keep.grass.Helpers;
using Xamarin.Forms;

namespace keep.grass
{
    public class AlphaTheme
    {
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

        private AlphaTheme()
        {
        }

        public static AlphaTheme White = new AlphaTheme
        {
            ForeGroundColor = Color.Black,
            BackGroundColor = Color.White,
        };
        public static AlphaTheme Black = new AlphaTheme
        {
            ForeGroundColor = Color.White,
            BackGroundColor = Color.Black,
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
        public static AlphaTheme Get()
        {
            return Get(Settings.Theme).Value;
        }
        public static void Set(String Theme)
        {
            Settings.Theme = Get(Theme).Key;
        }
    }
}

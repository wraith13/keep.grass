using System;
using System.Collections.Generic;
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

        public static AlphaTheme Light = new AlphaTheme
        {
            ForeGroundColor = Color.Black,
            BackGroundColor = Color.White,
        };
        public static AlphaTheme Dark = new AlphaTheme
        {
            ForeGroundColor = Color.White,
            BackGroundColor = Color.Black,
        };

        public static Dictionary<string, AlphaTheme> All = new Dictionary<string, AlphaTheme>
        {
            { nameof(Light), Light },
            { nameof(Dark), Dark },
        }
    }
}

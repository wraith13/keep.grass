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
        public Color ForegroundColor
        {
            get;
            private set;
        }
        public Color BackgroundColor
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
            ForegroundColor = Color.Black,
            BackgroundColor = Color.White,
        };
        public static AlphaTheme Black = new AlphaTheme
        {
            AccentColor = Color.Default,// Color.FromRgb(0x44, 0x55, 0xEE),
            ForegroundColor = Color.White,
            BackgroundColor = Color.Black,
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

        public static void Apply(object UIObject)
        {
            var Theme = Get();
            var ApplyHandler = UIObject as AlphaThemeApplyHandler;
            if (null != ApplyHandler)
            {
                ApplyHandler.ApplyTheme(Theme);
            }
            else
            {
				ApplyCore(UIObject, Theme);
				var AppliedHandler = UIObject as AlphaThemeAppliedHandler;
				if (null != ApplyHandler)
				{
					AppliedHandler.AppliedTheme(Theme);
				}
            }
        }
            private static void ApplyCore(object UIObject, AlphaTheme Theme)
            {
                //  C# 7.0 を早く・・・

                var ContentPage = UIObject as ContentPage;
                if (null != ContentPage)
                {
                    ContentPage.BackgroundColor = Theme.BackgroundColor;
                    Apply(ContentPage.Content);
                    return;
                }

                var Layout = UIObject as StackLayout;
                if (null != Layout)
                {
                    Layout.BackgroundColor = Theme.BackgroundColor;
                    foreach (var i in Layout.Children)
                    {
                        Apply(Layout.Children);
                    }
                    return;
                }
		}
    }
    interface AlphaThemeApplyHandler
	{
        void ApplyTheme(AlphaTheme Theme);
	}
	interface AlphaThemeAppliedHandler
	{
		void AppliedTheme(AlphaTheme Theme);
	}
	static class AlphaThemeHelper
    {
        public static void ApplyTheme(this View View, AlphaTheme Theme)
		{
            View.BackgroundColor = Theme.BackgroundColor;
		}
	}
}

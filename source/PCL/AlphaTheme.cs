using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using keep.grass.Helpers;
using Xamarin.Forms;
using RuyiJinguBang;

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
        public Func<double, Color> MakeLeftTimeColor
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
            ForegroundColor = Color.Default,
            BackgroundColor = Color.Default,
            MakeLeftTimeColor = LeftTimeRate => Color.FromRgb
            (
                r: (byte)(255.0 * (1.0 - LeftTimeRate)),
                g: (byte)(255.0 * Math.Min(0.5, LeftTimeRate)),
                b: 0
            ),
        };
        public static AlphaTheme Grass = new AlphaTheme
        {
            AccentColor = Color.Black,
            ForegroundColor = Color.FromRgb(0x10, 0x60, 0x20),
            BackgroundColor = Color.FromRgb(0x88, 0xEE, 0x99),
            MakeLeftTimeColor = LeftTimeRate => Color.FromRgb
            (
                r: (byte)(255.0 * (1.0 - LeftTimeRate)),
                g: (byte)(255.0 * Math.Min(0.5, LeftTimeRate)),
                b: 0
            ),
        };
        public static AlphaTheme Cherry = new AlphaTheme
        {
            AccentColor = Color.Red,
            ForegroundColor = Color.FromRgb(0x80, 0x40, 0x40),
            BackgroundColor = Color.FromRgb(0xFF, 0xCC, 0xCC),
            MakeLeftTimeColor = LeftTimeRate => Color.FromRgb
            (
                r: (byte)(255.0 * (1.0 - LeftTimeRate)),
                g: (byte)(255.0 * Math.Min(0.5, LeftTimeRate)),
                b: 0
            ),
        };
        public static AlphaTheme Abyss = new AlphaTheme
        {
            AccentColor = Color.Default,// Color.FromRgb(0x44, 0x55, 0xEE),
            ForegroundColor = Color.FromRgb(0xAA, 0xBB, 0xEE),
            BackgroundColor = Color.FromRgb(0x11, 0x33, 0x66),
            MakeLeftTimeColor = LeftTimeRate => Color.FromRgb
            (
                r: (byte)(255.0 * (1.0 - LeftTimeRate)),
                g: (byte)(255.0 * Math.Min(0.5, LeftTimeRate)),
                b: 0
            ),
        };
        public static AlphaTheme Black = new AlphaTheme
        {
            AccentColor = Color.Default,// Color.FromRgb(0x44, 0x55, 0xEE),
            ForegroundColor = Color.FromRgb(0xE8,0xF0,0xEC),
            BackgroundColor = Color.Black,
            MakeLeftTimeColor = LeftTimeRate => Color.FromRgb
            (
                r: (byte)(160.0 * (1.0 - LeftTimeRate)),
                g: (byte)(160.0 * Math.Min(0.5, LeftTimeRate)),
                b: (byte)40
            ),
        };

        public static Dictionary<string, AlphaTheme> All = new Dictionary<string, AlphaTheme>
        {
            { nameof(White), White },
            { nameof(Grass), Grass },
            { nameof(Cherry), Cherry },
            { nameof(Abyss), Abyss },
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
            var ApplyHandler = UIObject as IAlphaThemeApplyHandler;
            if (null != ApplyHandler)
            {
                ApplyHandler.ApplyTheme(Theme);
            }
            else
            {
				ApplyCore(UIObject, Theme);
            }
            var AppliedHandler = UIObject as IAlphaThemeAppliedHandler;
            if (null != ApplyHandler)
            {
                AppliedHandler.AppliedTheme(Theme);
            }
        }
        private static void ApplyCore(object UIObject, AlphaTheme Theme)
        {
            //  C# 7.0 を早く・・・

            var CircleGraph = UIObject as AlphaUserCircleGraph;
            if (null != CircleGraph)
            {
                //CircleGraph.AltTextColor = Theme.AccentColor;
                CircleGraph.BackgroundColor = Color.Default == Theme.BackgroundColor ?
                    Color.White:
                    Theme.BackgroundColor;
                return;
            }

            var ContentPage = UIObject as ContentPage;
            if (null != ContentPage)
            {
                ContentPage.BackgroundColor = Theme.BackgroundColor;
                Apply(ContentPage.Content);
                return;
            }

            var NavigationPage = UIObject as NavigationPage;
            if (null != NavigationPage)
            {
                NavigationPage.BarTextColor = Theme.ForegroundColor;
                NavigationPage.BarBackgroundColor = Theme.BackgroundColor;
                NavigationPage.BackgroundColor = Theme.BackgroundColor;
                Apply(NavigationPage.CurrentPage);
                return;
            }

            var Layout = UIObject as StackLayout;
            if (null != Layout)
            {
                Layout.BackgroundColor = Theme.BackgroundColor;
                foreach (var i in Layout.Children)
                {
					Debug.WriteLine($"i:{i.GetType().FullName}");
                    Apply(i);
                }
                return;
            }

            var Table = UIObject as TableView;
            if (null != Table)
            {
                Table.BackgroundColor = Theme.BackgroundColor;
                foreach (var i in Table.Root.AsEnumerable())
                {
                    Apply(i);
                }
                return;
            }

            var Section = UIObject as TableSection;
            if (null != Section)
            {
                foreach (var i in Section.AsEnumerable())
                {
                    Apply(i);
                }
                return;
            }

            var List = UIObject as ListView;
            if (null != List)
            {
                List.BackgroundColor = Theme.BackgroundColor;
                return;
            }

            var Label = UIObject as Label;
            if (null != Label)
            {
                Label.TextColor = Theme.ForegroundColor;
				Label.BackgroundColor = Theme.BackgroundColor;
                return;
            }

            var Button = UIObject as Button;
            if (null != Button)
            {
                Button.TextColor = Theme.AccentColor;
                Button.BackgroundColor = Theme.BackgroundColor;
                return;
            }

            var Grid = UIObject as Grid;
            if (null != Grid)
            {
                Grid.BackgroundColor = Theme.BackgroundColor;
                foreach (var i in Grid.Children)
                {
                    Debug.WriteLine($"i:{i.GetType().FullName}");
                    Apply(i);
                }
                return;
            }

            var ViewCell = UIObject as ViewCell;
            if (null != ViewCell)
            {
                Apply(ViewCell.View);
                return;
            }

            var View = UIObject as View;
            if (null != View)
            {
                View.BackgroundColor = Theme.BackgroundColor;
                return;
            }

        }
    }
    interface IAlphaThemeApplyHandler
    {
        void ApplyTheme(AlphaTheme Theme);
    }
    interface IAlphaThemeAppliedHandler
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

using Xamarin.Forms;
using RuyiJinguBang;
using keep.grass.Domain;
using System.Linq;

namespace keep.grass.App
{
    static class AlphaThemeStatic
    {
        public static void Apply(object UIObject)
        {
            Apply(UIObject, AlphaTheme.Get());
        }
        public static void Apply(object UIObject, AlphaTheme Theme)
        {
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
            if (null != AppliedHandler)
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
                CircleGraph.BackgroundColor = Color.Default == Theme.BackgroundColor.ToColor() ?
                    Color.White :
                    Theme.BackgroundColor.ToColor();
                return;
            }

            var ContentPage = UIObject as ContentPage;
            if (null != ContentPage)
            {
                ContentPage.BackgroundColor = Theme.BackgroundColor.ToColor();
                Apply(ContentPage.Content);
                return;
            }

            var NavigationPage = UIObject as NavigationPage;
            if (null != NavigationPage)
            {
                NavigationPage.BarTextColor = Theme.ForegroundColor.ToColor();
                NavigationPage.BarBackgroundColor = Theme.BackgroundColor.ToColor();
                NavigationPage.BackgroundColor = Theme.BackgroundColor.ToColor();
                Apply(NavigationPage.CurrentPage);
                return;
            }

            var Layout = UIObject as StackLayout;
            if (null != Layout)
            {
                Layout.BackgroundColor = Theme.BackgroundColor.ToColor();
                foreach (var i in Layout.Children)
                {
                    //Debug.WriteLine($"i:{i.GetType().FullName}");
                    Apply(i);
                }
                return;
            }

            var Table = UIObject as TableView;
            if (null != Table)
            {
                Table.BackgroundColor = Theme.BackgroundColor.ToColor();
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
                List.BackgroundColor = Theme.BackgroundColor.ToColor();
                return;
            }

            var Search = UIObject as SearchBar;
            if (null != Search)
            {
                return;
            }

            var Label = UIObject as Label;
            if (null != Label)
            {
                Label.TextColor = Theme.ForegroundColor.ToColor();
                Label.BackgroundColor = Theme.BackgroundColor.ToColor();
                return;
            }

            var Button = UIObject as Button;
            if (null != Button)
            {
                Button.TextColor = Theme.AccentColor.ToColor();
                Button.BackgroundColor = Theme.BackgroundColor.ToColor();
                return;
            }

            var Grid = UIObject as Grid;
            if (null != Grid)
            {
                Grid.BackgroundColor = Theme.BackgroundColor.ToColor();
                foreach (var i in Grid.Children)
                {
                    //Debug.WriteLine($"i:{i.GetType().FullName}");
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
                View.BackgroundColor = Theme.BackgroundColor.ToColor();
                return;
            }
        }
    }
    public interface IAlphaThemeApplyHandler
    {
        void ApplyTheme(AlphaTheme Theme);
    }
    public interface IAlphaThemeAppliedHandler
    {
        void AppliedTheme(AlphaTheme Theme);
    }
    public static class AlphaThemeHelper
    {
        public static void ApplyTheme(this View View, AlphaTheme Theme)
        {
            View.BackgroundColor = Theme.BackgroundColor.ToColor();
        }
    }
}

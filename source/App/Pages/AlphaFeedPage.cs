using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using RuyiJinguBang;
using keep.grass.Domain;

namespace keep.grass.App
{
    //  Activity だと Android のそれと紛らわしいので Feed とした。
    public class AlphaFeedPage : ResponsiveContentPage
    {
        AlphaDomain Domain = AlphaDomainFactory.MakeSureDomain();
        AlphaLanguage L = AlphaDomainFactory.MakeSureLanguage();

        public GitHub.Feed Feed;

        public AlphaFeedPage(string User)
        {
            Title = L["Activity"];
            Domain.GetFeed(User).ContinueWith
            (
                t => Device.BeginInvokeOnMainThread
                (
                    () =>
                    {
                        if (null == t.Exception)
                        {
                            Feed = t.Result;
                            Build();
                            AlphaThemeStatic.Apply(this);
                        }
                        else
                        {
                            Debug.WriteLine(t.Exception);
                            AlphaAppFactory.MakeSureApp().Navigation.PopAsync();
                        }
                    }
                )
            );
        }
        public override void Build()
        {
            base.Build();
            Debug.WriteLine("AlphaFeedPage.Rebuild();");
            if (null == Feed)
            {
                Content = new ActivityIndicator
                {
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    IsRunning = true,
                };
            }
            else
            {
                Content = new ListView
                {
                    HasUnevenRows = true,
                    ItemTemplate = new DataTemplateEx(AlphaAppFactory.GetFeedEntryCellType()).SetBindingList("Entry"),
                    ItemsSource = Feed?.EntryList?.Select(i => new { Entry = i, }),
                };
            }
            AlphaThemeStatic.Apply(this);
        }
    }
}


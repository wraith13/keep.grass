﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using Xamarin.Forms;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;
using keep.grass.Domain;

namespace keep.grass.App
{
    public abstract class AlphaApp : Application
    {
        public NavigationPage Navigation;
        public AlphaMainPage Main;
        AlphaDomain Domain = AlphaDomainFactory.MakeSureDomain();

        public AlphaApp()
        {
            AlphaAppFactory.SetApp(this);

            MainPage = Navigation = new NavigationPage
            (
                Main = AlphaAppFactory.MakeMainPage()
            );
            MainPage.Title = "keep.grass";
            AlphaThemeStatic.Apply(Navigation);

            Domain.OnStartQuery = () =>
                Xamarin.Forms.Device.BeginInvokeOnMainThread
                (
                    () =>
                    {
                        AlphaAppFactory.GetApp()?.Main?.OnStartQuery();
                    }
                );
            Domain.OnUpdateLastPublicActivity = (string User, DateTime LastPublicActivity) =>
                Xamarin.Forms.Device.BeginInvokeOnMainThread
                (
                    () =>
                    {
                        if (User == Settings.UserName)
                        {
                            Domain.UpdateAlerts(LastPublicActivity);
                        }
                        AlphaAppFactory.GetApp()?.Main?.OnUpdateLastPublicActivity(User, LastPublicActivity);
                    }
                );
            Domain.OnUpdateIcon = (string User, byte[] Binary) =>
                Xamarin.Forms.Device.BeginInvokeOnMainThread
                (
                    () =>
                    {
                        AlphaAppFactory.GetApp()?.Main?.OnUpdateIcon(User, Binary);
                    }
                );
            Domain.OnErrorInQuery = () =>
                Xamarin.Forms.Device.BeginInvokeOnMainThread
                (
                    () =>
                    {
                        AlphaAppFactory.GetApp()?.Main?.OnErrorInQuery();
                    }
                );
            Domain.OnEndQuery = () =>
                Xamarin.Forms.Device.BeginInvokeOnMainThread
                (
                    () =>
                    {
                        AlphaAppFactory.GetApp()?.Main?.OnEndQuery();
                    }
                );
        }

        public void RebuildMainPage()
        {
            Debug.WriteLine("AlphaApp::RebuildMainPage");
            Main.Build();
            AlphaThemeStatic.Apply(Navigation);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            base.OnStart();

            // Mobile Center 初期化処理。(もしビルドエラーになるなら取り敢えず「Properties.KeySample.Mobi略」にすれば通る)
            AppCenter.Start(
                appSecret:
                    $"ios={keep.grass.Properties.Key.MobileCenterSecretIos};"
                    + $"android={keep.grass.Properties.Key.MobileCenterSecretAndroid};"
                    + $"uwp={keep.grass.Properties.Key.MobileCenterSecretUwp};",
                services: new[] { typeof(Analytics), typeof(Crashes), typeof(Push), }
            );

            // Mobile Center クラッシュのデータ収集を有効化する
            Crashes.SetEnabledAsync(true);
        }
        protected override void OnSleep()
        {
            Main.OnPause();
        }
        protected override void OnResume()
        {
            Domain.UpdateAlerts(Domain.GetLastPublicActivity(Settings.UserName));
        }

        public void ShowMainPage()
        {
            Navigation.PopToRootAsync();
        }

        public void ShowDetailPage(string User)
        {
            Analytics.TrackEvent(
                name: "[Clicked] UserGraph",
                properties: new Dictionary<string, string>
                {
                    { "Category", "GraphClick" },
                    { "Screen", "MainPage" }
                }
            );
            Navigation.PushAsync(new AlphaDetailPage(User));
        }

        public void ShowFeedPage(string User)
        {
            Analytics.TrackEvent(
                name: "[Clicked] LastActivity",
                properties: new Dictionary<string, string>
                {
                    { "Category", "ColumnClick" },
                    { "Screen", "DetailPage" }
                }
            );
            Navigation.PushAsync(new AlphaFeedPage(User));
        }

        public void ShowSettingsPage()
        {
            // Mobile Center .Analytics にデータ送信
            Analytics.TrackEvent(
                name: "[Clicked] Setting Button",
                properties: new Dictionary<string, string>
                {
                    { "Category", "ButtonClick" },
                    { "Screen", "MainPage" }
                }
            );
            // ページ遷移処理
            Navigation.PushAsync(AlphaAppFactory.MakeSettingsPage());
        }

        public void ShowSelectUserPage(Action<string> Reciever, IEnumerable<string> ExistUsers = null)
        {
            Navigation.PushAsync(AlphaAppFactory.MakeSelectUserPage(Reciever, ExistUsers));
        }

        public virtual void OnChangeSettings()
        {
            AlphaDomainFactory.MakeSureLanguage().Update();
            Main.UpdateInfoAsync();
            Domain.UpdateAlerts(Domain.GetLastPublicActivity(Settings.UserName));
        }

        public virtual ImageSource GetImageSource(string image)
        {
            return ImageSource.FromResource
            (
                "keep.grass.App.Images." + image,
                typeof(AlphaApp).GetTypeInfo().Assembly
            );
        }
        public virtual ImageSource GetApplicationImageSource()
        {
            return GetImageSource("keep.grass.120.png");
        }
        public virtual ImageSource GetWraithImageSource()
        {
            return GetImageSource("wraith13.120.png");
        }
        public virtual ImageSource GetGitHubImageSource()
        {
            return GetImageSource("GitHub-Mark.120.png");
        }
        public virtual ImageSource GetRightImageSource()
        {
            return GetImageSource("right.120.png");
        }
        public virtual ImageSource GetRefreshImageSource()
        {
            return GetImageSource("refresh.120.png");
        }
        public virtual ImageSource GetExportImageSource()
        {
            return GetImageSource("export.120.png");
        }
        public virtual ImageSource GetOcticonImageSource(string Name)
        {
            var Tag = "octicon-";
            if (Name?.StartsWith(Tag) ?? false)
            {
                return GetOcticonImageSource(Name.Substring(Tag.Length));
            }
            else
            {
                switch (Name)
                {
                case "book":
                case "comment-discussion":
                case "git-branch":
                case "git-commit":
                case "git-compare":
                case "git-merge":
                case "git-pull-request":
                case "issue-closed":
                case "issue-opened":
                case "issue-reopened":
                case "mark-github":
                case "person":
                case "repo":
                case "star":
                case "tag":
                    return GetImageSource($"octicons.{Name}.png");
                }
            }
            Debug.WriteLine($"GetOcticonImageSource({Name}): NOT FOUND ICON!!!");
            return GetImageSource("octicons.mark-github.png");
        }
        public virtual Stream GetFontStream()
        {
            return typeof(AlphaApp).GetTypeInfo().Assembly.GetManifestResourceStream("keep.grass.App.Fonts.NotoSansCJKjp-Regular.otf");
        }
    }
}


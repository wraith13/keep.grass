﻿using System;
using System.Diagnostics;
using Xamarin.Forms;
using RuyiJinguBang;
using keep.grass.Domain;

namespace keep.grass.App
{
    public class AlphaInfoPage : ResponsiveContentPage
    {
        AlphaApp Root = AlphaAppFactory.MakeSureApp();
        AlphaLanguage L = AlphaDomainFactory.MakeSureLanguage();

        public AlphaInfoPage()
        {
            Title = L["Information"];
        }
        public override void Build()
        {
            base.Build();
            Debug.WriteLine("AlphaInfoPage.Rebuild();");

            var Version = new TableSection(L["Version"])
            {
                AlphaAppFactory.MakeCircleImageCell
                (
                    ImageSource: Root.GetApplicationImageSource(),
                    Text: "2.00.005",
                    Command: new Command
                    (
                        o => Device.OpenUri
                        (
                            AlphaDomainFactory.MakeSureDomain().GetApplicationStoreUri()
                        )
                    ),
                    OptionImageSource: Root.GetExportImageSource()
                ),
            };
            var Auther = new TableSection(L["Auther"])
            {
                AlphaAppFactory.MakeCircleImageCell
                (
                    ImageSource: Root.GetWraithImageSource(),
                    Text: "@wraith13",
                    Command: new Command(o => Device.OpenUri(new Uri("https://twitter.com/wraith13"))),
                    OptionImageSource: Root.GetExportImageSource()
                ),
            };
            var Repository = new TableSection(L["Github Repository"])
            {
                AlphaAppFactory.MakeCircleImageCell
                (
                    ImageSource: Root.GetGitHubImageSource(),
                    Text: "wraith13/keep.grass",
                    Command: new Command(o => Device.OpenUri(new Uri("https://github.com/wraith13/keep.grass"))),
                    OptionImageSource: Root.GetExportImageSource()
                ),
            };
            var BuiltWith = new TableSection(L["Built with"])
            {
                AlphaAppFactory.MakeCircleImageCell
                (
                    ImageSource: null,
                    Text: "Xamarin",
                    Command: new Command(o => Device.OpenUri(new Uri("https://www.xamarin.com"))),
                    OptionImageSource: Root.GetExportImageSource()
                ),
                AlphaAppFactory.MakeCircleImageCell
                (
                    ImageSource: null,
                    Text: "Visual Studio",
                    Command: new Command(o => Device.OpenUri(new Uri("https://www.visualstudio.com/vs/"))),
                    OptionImageSource: Root.GetExportImageSource()
                ),
                AlphaAppFactory.MakeCircleImageCell
                (
                    ImageSource: null,
                    Text: "Visual Studio Code",
                    Command: new Command(o => Device.OpenUri(new Uri("https://code.visualstudio.com/"))),
                    OptionImageSource: Root.GetExportImageSource()
                ),
                AlphaAppFactory.MakeCircleImageCell
                (
                    ImageSource: null,
                    Text: "GIMP",
                    Command: new Command(o => Device.OpenUri(new Uri("https://www.gimp.org"))),
                    OptionImageSource: Root.GetExportImageSource()
                ),
                AlphaAppFactory.MakeCircleImageCell
                (
                    ImageSource: null,
                    Text: "Microsoft HTTP Client Lib.",
                    Command: new Command(o => Device.OpenUri(new Uri("https://www.nuget.org/packages/Microsoft.Net.Http/"))),
                    OptionImageSource: Root.GetExportImageSource()
                ),
                AlphaAppFactory.MakeCircleImageCell
                (
                    ImageSource: null,
                    Text: "Microsoft.Azure.Mobile.Analytics",
                    Command: new Command(o => Device.OpenUri(new Uri("https://www.nuget.org/packages/Microsoft.Azure.Mobile.Analytics/"))),
                    OptionImageSource: Root.GetExportImageSource()
                ),
                AlphaAppFactory.MakeCircleImageCell
                (
                    ImageSource: null,
                    Text: "Microsoft.Azure.Mobile.Crashes",
                    Command: new Command(o => Device.OpenUri(new Uri("https://www.nuget.org/packages/Microsoft.Azure.Mobile.Crashes//"))),
                    OptionImageSource: Root.GetExportImageSource()
                ),
                AlphaAppFactory.MakeCircleImageCell
                (
                    ImageSource: null,
                    Text: "Settings Plugin",
                    Command: new Command(o => Device.OpenUri(new Uri("https://github.com/jamesmontemagno/SettingsPlugin"))),
                    OptionImageSource: Root.GetExportImageSource()
                ),
                AlphaAppFactory.MakeCircleImageCell
                (
                    ImageSource: null,
                    Text: "Circle Image Control Plugin",
                    Command: new Command(o => Device.OpenUri(new Uri("https://github.com/jamesmontemagno/ImageCirclePlugin"))),
                    OptionImageSource: Root.GetExportImageSource()
                ),
                AlphaAppFactory.MakeCircleImageCell
                (
                    ImageSource: null,
                    Text: "NotificationsExtensions",
                    Command: new Command(o => Device.OpenUri(new Uri("https://github.com/WindowsNotifications/NotificationsExtensions"))),
                    OptionImageSource: Root.GetExportImageSource()
                ),
                AlphaAppFactory.MakeCircleImageCell
                (
                    ImageSource: null,
                    Text: "SkiaSharp(.Views.Forms)",
                    Command: new Command(o => Device.OpenUri(new Uri("https://github.com/mono/SkiaSharp"))),
                    OptionImageSource: Root.GetExportImageSource()
                ),
                AlphaAppFactory.MakeCircleImageCell
                (
                    ImageSource: null,
                    Text: "Json.NET",
                    Command: new Command(o => Device.OpenUri(new Uri("http://www.newtonsoft.com/json"))),
                    OptionImageSource: Root.GetExportImageSource()
                ),
                AlphaAppFactory.MakeCircleImageCell
                (
                    ImageSource: null,
                    Text: "Noto Sans CJK jp Regular",
                    Command: new Command(o => Device.OpenUri(new Uri("https://www.google.com/get/noto/help/cjk/"))),
                    OptionImageSource: Root.GetExportImageSource()
                ),
                AlphaAppFactory.MakeCircleImageCell
                (
                    ImageSource: null,
                    Text: "GitHub Octicons",
                    Command: new Command(o => Device.OpenUri(new Uri("https://octicons.github.com"))),
                    OptionImageSource: Root.GetExportImageSource()
                ),
            };

            var StackContent = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 1.0,
                BackgroundColor = Color.Gray,
            };
            if (Width <= Height)
            {
                StackContent.Children.Add
                (
                    new TableView
                    {
                        BackgroundColor = Color.White,
                        Root = new TableRoot
                            {
                                Version,
                                Auther,
                                Repository,
                                BuiltWith,
                            }
                    }
               );
            }
            else
            {
                StackContent.Children.Add
                (
                    new TableView
                    {
                        BackgroundColor = Color.White,
                        Root = new TableRoot
                        {
                            Version,
                            Auther,
                            Repository,
                        }
                    }
               );
                StackContent.Children.Add
                (
                    new TableView
                    {
                        BackgroundColor = Color.White,
                        Root = new TableRoot
                        {
                            BuiltWith,
                        }
                    }
                );
            }
            Content = StackContent;
            AlphaThemeStatic.Apply(this);
        }
    }
}


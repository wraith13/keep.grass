using System;
using System.Diagnostics;
using Xamarin.Forms;
using RuyiJinguBang;

namespace keep.grass
{
    public class AlphaInfoPage : ResponsiveContentPage
    {
        AlphaApp Root = AlphaFactory.MakeSureApp();
        Languages.AlphaLanguage L = AlphaFactory.MakeSureLanguage();

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
                AlphaFactory.MakeCircleImageCell
                (
                    ImageBytes: Root.GetApplicationImageSource(),
                    Text: "2.00.004",
                    Command: new Command
                    (
                        o => Device.OpenUri
                        (
                            AlphaFactory.MakeSureDomain().GetApplicationStoreUri()
                        )
                    ),
                    OptionImageBytes: Root.GetExportImageSource()
                ),
            };
            var Auther = new TableSection(L["Auther"])
            {
                AlphaFactory.MakeCircleImageCell
                (
                    ImageBytes: Root.GetWraithImageSource(),
                    Text: "@wraith13",
                    Command: new Command(o => Device.OpenUri(new Uri("https://twitter.com/wraith13"))),
                    OptionImageBytes: Root.GetExportImageSource()
                ),
            };
            var Repository = new TableSection(L["Github Repository"])
            {
                AlphaFactory.MakeCircleImageCell
                (
                    ImageBytes: Root.GetGitHubImageSource(),
                    Text: "wraith13/keep.grass",
                    Command: new Command(o => Device.OpenUri(new Uri("https://github.com/wraith13/keep.grass"))),
                    OptionImageBytes: Root.GetExportImageSource()
                ),
            };
            var BuiltWith = new TableSection(L["Built with"])
            {
                AlphaFactory.MakeCircleImageCell
                (
                    ImageBytes: null,
                    Text: "Xamarin",
                    Command: new Command(o => Device.OpenUri(new Uri("https://www.xamarin.com"))),
                    OptionImageBytes: Root.GetExportImageSource()
                ),
                AlphaFactory.MakeCircleImageCell
                (
                    ImageBytes: null,
                    Text: "Visual Studio",
                    Command: new Command(o => Device.OpenUri(new Uri("https://www.visualstudio.com/vs/"))),
                    OptionImageBytes: Root.GetExportImageSource()
                ),
                AlphaFactory.MakeCircleImageCell
                (
                    ImageBytes: null,
                    Text: "Visual Studio Code",
                    Command: new Command(o => Device.OpenUri(new Uri("https://code.visualstudio.com/"))),
                    OptionImageBytes: Root.GetExportImageSource()
                ),
                AlphaFactory.MakeCircleImageCell
                (
                    ImageBytes: null,
                    Text: "GIMP",
                    Command: new Command(o => Device.OpenUri(new Uri("https://www.gimp.org"))),
                    OptionImageBytes: Root.GetExportImageSource()
                ),
                AlphaFactory.MakeCircleImageCell
                (
                    ImageBytes: null,
                    Text: "Microsoft HTTP Client Lib.",
                    Command: new Command(o => Device.OpenUri(new Uri("https://www.nuget.org/packages/Microsoft.Net.Http/"))),
                    OptionImageBytes: Root.GetExportImageSource()
                ),
                AlphaFactory.MakeCircleImageCell
                (
                    ImageBytes: null,
                    Text: "Microsoft.Azure.Mobile.Analytics",
                    Command: new Command(o => Device.OpenUri(new Uri("https://www.nuget.org/packages/Microsoft.Azure.Mobile.Analytics/"))),
                    OptionImageBytes: Root.GetExportImageSource()
                ),
                AlphaFactory.MakeCircleImageCell
                (
                    ImageBytes: null,
                    Text: "Microsoft.Azure.Mobile.Crashes",
                    Command: new Command(o => Device.OpenUri(new Uri("https://www.nuget.org/packages/Microsoft.Azure.Mobile.Crashes//"))),
                    OptionImageBytes: Root.GetExportImageSource()
                ),
                AlphaFactory.MakeCircleImageCell
                (
                    ImageBytes: null,
                    Text: "Settings Plugin",
                    Command: new Command(o => Device.OpenUri(new Uri("https://github.com/jamesmontemagno/SettingsPlugin"))),
                    OptionImageBytes: Root.GetExportImageSource()
                ),
                AlphaFactory.MakeCircleImageCell
                (
                    ImageBytes: null,
                    Text: "Circle Image Control Plugin",
                    Command: new Command(o => Device.OpenUri(new Uri("https://github.com/jamesmontemagno/ImageCirclePlugin"))),
                    OptionImageBytes: Root.GetExportImageSource()
                ),
                AlphaFactory.MakeCircleImageCell
                (
                    ImageBytes: null,
                    Text: "NotificationsExtensions",
                    Command: new Command(o => Device.OpenUri(new Uri("https://github.com/WindowsNotifications/NotificationsExtensions"))),
                    OptionImageBytes: Root.GetExportImageSource()
                ),
                AlphaFactory.MakeCircleImageCell
                (
                    ImageBytes: null,
                    Text: "SkiaSharp(.Views.Forms)",
                    Command: new Command(o => Device.OpenUri(new Uri("https://github.com/mono/SkiaSharp"))),
                    OptionImageBytes: Root.GetExportImageSource()
                ),
                AlphaFactory.MakeCircleImageCell
                (
                    ImageBytes: null,
                    Text: "Json.NET",
                    Command: new Command(o => Device.OpenUri(new Uri("http://www.newtonsoft.com/json"))),
                    OptionImageBytes: Root.GetExportImageSource()
                ),
                AlphaFactory.MakeCircleImageCell
                (
                    ImageBytes: null,
                    Text: "Noto Sans CJK jp Regular",
                    Command: new Command(o => Device.OpenUri(new Uri("https://www.google.com/get/noto/help/cjk/"))),
                    OptionImageBytes: Root.GetExportImageSource()
                ),
                AlphaFactory.MakeCircleImageCell
                (
                    ImageBytes: null,
                    Text: "GitHub Octicons",
                    Command: new Command(o => Device.OpenUri(new Uri("https://octicons.github.com"))),
                    OptionImageBytes: Root.GetExportImageSource()
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
            AlphaTheme.Apply(this);
        }
    }
}


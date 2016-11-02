using System;
using System.Diagnostics;
using Xamarin.Forms;

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
					ImageSource: Root.GetApplicationImageSource(),
					Text: "1.00.002",
					Command: new Command
					(
						o => Device.OpenUri
						(
							AlphaFactory.MakeSureDomain().GetApplicationStoreUri()
						)
					),
					OptionImageSource: Root.GetExportImageSource()
				),
			};
			var Auther = new TableSection(L["Auther"])
			{
				AlphaFactory.MakeCircleImageCell
				(
					ImageSource: Root.GetWraithImageSource(),
					Text: "@wraith13",
					Command: new Command(o => Device.OpenUri(new Uri("https://twitter.com/wraith13"))),
					OptionImageSource: Root.GetExportImageSource()
				),
			};
			var Repository = new TableSection(L["Github Repository"])
			{
				AlphaFactory.MakeCircleImageCell
				(
					ImageSource: Root.GetGitHubImageSource(),
					Text: "wraith13/keep.grass",
					Command: new Command(o => Device.OpenUri(new Uri("https://github.com/wraith13/keep.grass"))),
					OptionImageSource: Root.GetExportImageSource()
				),
			};
			var BuiltWith = new TableSection(L["Built with"])
			{
				AlphaFactory.MakeCircleImageCell
				(
					ImageSource: null,
					Text: "Xamarin",
					Command: new Command(o => Device.OpenUri(new Uri("https://www.xamarin.com"))),
					OptionImageSource: Root.GetExportImageSource()
				),
				AlphaFactory.MakeCircleImageCell
				(
					ImageSource: null,
					Text: "Visual Studio",
					Command: new Command(o => Device.OpenUri(new Uri("https://www.visualstudio.com/vs/"))),
					OptionImageSource: Root.GetExportImageSource()
				),
				AlphaFactory.MakeCircleImageCell
				(
					ImageSource: null,
					Text: "Visual Studio Code",
					Command: new Command(o => Device.OpenUri(new Uri("https://code.visualstudio.com/"))),
					OptionImageSource: Root.GetExportImageSource()
				),
				AlphaFactory.MakeCircleImageCell
				(
					ImageSource: null,
					Text: "GIMP",
					Command: new Command(o => Device.OpenUri(new Uri("https://www.gimp.org"))),
					OptionImageSource: Root.GetExportImageSource()
				),
				AlphaFactory.MakeCircleImageCell
				(
					ImageSource: null,
					Text: "Microsoft HTTP Client Lib.",
					Command: new Command(o => Device.OpenUri(new Uri("https://www.nuget.org/packages/Microsoft.Net.Http/"))),
					OptionImageSource: Root.GetExportImageSource()
				),
				AlphaFactory.MakeCircleImageCell
				(
					ImageSource: null,
					Text: "Settings Plugin",
					Command: new Command(o => Device.OpenUri(new Uri("https://github.com/jamesmontemagno/SettingsPlugin"))),
					OptionImageSource: Root.GetExportImageSource()
				),
				AlphaFactory.MakeCircleImageCell
				(
					ImageSource: null,
					Text: "Circle Image Control Plugin",
					Command: new Command(o => Device.OpenUri(new Uri("https://github.com/jamesmontemagno/ImageCirclePlugin"))),
					OptionImageSource: Root.GetExportImageSource()
				),
				AlphaFactory.MakeCircleImageCell
				(
					ImageSource: null,
					Text: "NotificationsExtensions",
					Command: new Command(o => Device.OpenUri(new Uri("https://github.com/WindowsNotifications/NotificationsExtensions"))),
					OptionImageSource: Root.GetExportImageSource()
				),
				AlphaFactory.MakeCircleImageCell
				(
					ImageSource: null,
					Text: "SkiaSharp(.Views.Forms)",
					Command: new Command(o => Device.OpenUri(new Uri("https://github.com/mono/SkiaSharp"))),
					OptionImageSource: Root.GetExportImageSource()
				),
				AlphaFactory.MakeCircleImageCell
				(
					ImageSource: null,
					Text: "Noto Sans CJK jp Regular",
					Command: new Command(o => Device.OpenUri(new Uri("https://www.google.com/get/noto/help/cjk/"))),
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
		}
	}
}


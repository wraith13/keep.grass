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
					Text: "1.00.000",
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

			Content = new StackLayout
			{
				Children =
				{
					new TableView
					{
						Root = new TableRoot
						{
							Version,
							Auther,
							Repository,
						}
					},
				},
			};
		}
	}
}


using System;
using Xamarin.Forms;

namespace keep.grass
{
	public class AlphaInfoPage : ContentPage
	{
		public AlphaInfoPage()
		{
			AlphaApp Root = AlphaFactory.MakeSureApp();
			Languages.AlphaLanguage L = AlphaFactory.MakeSureLanguage();

			Title = L["Information"];
			Content = new StackLayout
			{
				Children =
				{
					new TableView
					{
						Root = new TableRoot
						{
							new TableSection(L["Version"])
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
									)
								),
							},
							new TableSection(L["Auther"])
							{
								AlphaFactory.MakeCircleImageCell
								(
									ImageSource: Root.GetWraithImageSource(),
									Text: "@wraith13",
									Command: new Command(o => Device.OpenUri(new Uri("https://twitter.com/wraith13")))
								),
							},
							new TableSection(L["Github Repository"])
							{
								AlphaFactory.MakeCircleImageCell
								(
									ImageSource: Root.GetGitHubImageSource(),
									Text: "wraith13/keep.grass",
									Command: new Command(o => Device.OpenUri(new Uri("https://github.com/wraith13/keep.grass")))
								),
							},
						}
					},
				},
			};
		}
	}
}


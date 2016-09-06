using System;
using Xamarin.Forms;

namespace keep.grass
{
	public class AlphaInfoPage : ContentPage
	{
		AlphaApp Root = AlphaFactory.MakeSureApp();
		Languages.AlphaLanguage L = AlphaFactory.MakeSureLanguage();

		public AlphaInfoPage()
		{
			Title = L["Information"];

			var version = AlphaFactory.MakeCircleImageCell();
			version.ImageSource = Root.GetApplicationImageSource();
			version.Text = "1.00.000";
			version.Command = new Command
			(
				o => Device.OpenUri
				(
					AlphaFactory.MakeSureDomain().GetApplicationStoreUri()
				)
			);

			var twitter = AlphaFactory.MakeCircleImageCell();
			twitter.ImageSource = Root.GetWraithImageSource();
			twitter.Text = "@wraith13";
			twitter.Command = new Command(o => Device.OpenUri(new Uri("https://twitter.com/wraith13")));

			var github = AlphaFactory.MakeCircleImageCell();
			github.ImageSource = Root.GetGitHubImageSource();
			github.Text = "wraith13/keep.grass";
			github.Command = new Command(o => Device.OpenUri(new Uri("https://github.com/wraith13/keep.grass")));

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
								version
							},
							new TableSection(L["Auther"])
							{
								twitter
							},
							new TableSection(L["Github Repository"])
							{
								github
							},
						}
					},
				},
			};
		}
	}
}


using System;
using Xamarin.Forms;

namespace keep.grass
{
	public class AlphaInfoPage : ContentPage
	{
		AlphaApp Root;
		public Languages.AlphaLanguage L;

		public AlphaInfoPage(AlphaApp AppRoot)
		{
			Root = AppRoot;
			L = Root.L;
			Title = L["information"];

			var version = AlphaFactory.MakeCircleImageCell();
			version.ImageSource = "https://raw.githubusercontent.com/wraith13/keep.grass/master/source/iOS/Resources/Images.xcassets/AppIcons.appiconset/keep.grass.180.png";
			version.Text = "1.00.000";

			var twitter = AlphaFactory.MakeCircleImageCell();
			twitter.ImageSource = GitHub.GetIconUrl("wraith13");
			twitter.Text = "@wraith13";
			twitter.Command = new Command(o => Device.OpenUri(new Uri("https://twitter.com/wraith13")));

			var github = AlphaFactory.MakeCircleImageCell();
			github.ImageSource = "https://assets-cdn.github.com/images/modules/logos_page/GitHub-Mark.png";
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


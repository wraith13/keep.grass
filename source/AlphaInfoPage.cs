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
			var twitter = AlphaFactory.MakeCircleImageCell();
			twitter.ImageSource = GitHub.GetIconUrl("wraith13");
			twitter.Text = "@wraith13";
			twitter.Command = new Command(o => Device.OpenUri(new Uri("https://twitter.com/wraith13")));

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
								new TextCell
								{
									Text = "1.00.000",
								}
							},
							new TableSection(L["Auther"])
							{
								twitter
							},
							new TableSection(L["Github Repository"])
							{
								new TextCell
								{
									Text = "https://github.com/wraith13/keep.grass",
								}
							},
						}
					},
				},
			};
		}
	}
}


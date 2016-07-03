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
								new TextCell
								{
									Text = "https://twitter.com/wraith13",
								}
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


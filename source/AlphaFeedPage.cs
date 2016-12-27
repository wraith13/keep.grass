using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace keep.grass
{
	//	Activity だと Android のそれと紛らわしいので Feed とした。
	public class AlphaFeedPage : ResponsiveContentPage
	{
		AlphaDomain Domain = AlphaFactory.MakeSureDomain();
		Languages.AlphaLanguage L = AlphaFactory.MakeSureLanguage();

		public String User;
		public GitHub.Feed Feed;

		public AlphaFeedPage(string UserName)
		{
			User = UserName;
			Title = L["Activity"];
			Task.Run
			(
				async () =>
				{
					Feed = await Domain.GetFeed(User);
					Device.BeginInvokeOnMainThread
					(
						() =>
						{
							Build();
						}
					);
				}
			);
		}
		public override void Build()
		{
			base.Build();
			Debug.WriteLine("AlphaFeedPage.Rebuild();");

			Content = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Spacing = 1.0,
				BackgroundColor = Color.Gray,
				Children =
				{
					new TableView
					{
						BackgroundColor = Color.White,
						Root = new TableRoot
						{
							//new TableSection(Feed?.Title ?? L["Activity"])
							new TableSection()
							{
								Feed?.EntryList?.Select
								(
									i => new AlphaFeedEntryCell()
									{
										Entry = i
									}
								) ?? new AlphaFeedEntryCell[] {}
							},
						}
					},
				}
			};
		}
	}
}


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

		public GitHub.Feed Feed;

		public AlphaFeedPage(string User)
		{
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
			Content = new ListView
			{
				HasUnevenRows = true,
				ItemTemplate = new DataTemplateEx(typeof(AlphaFeedEntryCell)).SetBindingList("Entry"),
				ItemsSource = Feed?.EntryList?.Select(i => new { Entry = i, }),
			};
		}
	}
}


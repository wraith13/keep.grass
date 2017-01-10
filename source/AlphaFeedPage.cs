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
			Domain.GetFeed(User).ContinueWith
			(
				t => Device.BeginInvokeOnMainThread
				(
					() =>
					{
						if (null == t.Exception)
						{
							Feed = t.Result;
							Build();
						}
						else
						{
							Debug.WriteLine(t.Exception);
							AlphaFactory.MakeSureApp().Navigation.PopAsync();
						}
					}
				)
			);
		}
		public override void Build()
		{
			base.Build();
			Debug.WriteLine("AlphaFeedPage.Rebuild();");
			if (null == Feed)
			{
				Content = new ActivityIndicator
				{
					VerticalOptions = LayoutOptions.CenterAndExpand,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
					IsRunning = true,
				};
			}
			else
			{
				Content = new ListView
				{
					HasUnevenRows = true,
					ItemTemplate = new DataTemplateEx(AlphaFactory.GetFeedEntryCellType()).SetBindingList("Entry"),
					ItemsSource = Feed?.EntryList?.Select(i => new { Entry = i, }),
				};
			}
		}
	}
}


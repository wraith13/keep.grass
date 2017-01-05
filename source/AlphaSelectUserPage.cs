using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace keep.grass
{
	public class AlphaSelectUserPage : ResponsiveContentPage
	{
		AlphaDomain Domain = AlphaFactory.MakeSureDomain();
		Languages.AlphaLanguage L = AlphaFactory.MakeSureLanguage();

		public Action<string> Reciever;

		public AlphaSelectUserPage(Action<string> aReciever)
		{
			Reciever = aReciever;
			Title = L["Select User"];

			Content = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Spacing = 1.0,
				BackgroundColor = Color.Gray,
				Children =
				{
					new SearchBar
					{
						Placeholder = "ユーザーの名前等",
						//SearchCommand = 
					},
					new ListView
					{
					},
				}
			};
		}
		public override void Build()
		{
			base.Build();
			Debug.WriteLine("AlphaSelectUserPage.Rebuild();");

		}
	}
}


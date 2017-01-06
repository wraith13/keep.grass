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

		ListView List;
		SearchBar Search;
		ActivityIndicator Indicator;

		public AlphaSelectUserPage(Action<string> aReciever)
		{
			Reciever = aReciever;
			Title = L["Select User"];

			List = new ListView
			{
				ItemTemplate = new DataTemplateEx(typeof(AlphaCircleImageCell))
					.SetBinding("ImageSource", "AvatarUrl")
					.SetBinding("Text", "Login"),
			};
			List.ItemTapped += (sender, e) =>
			{
				var User = e.Item as GitHub.SearchUser;
				if (null != User)
				{
					Reciever(User.Login);
				}
				AlphaFactory.MakeSureApp().Navigation.PopAsync();
			};

			Search = new SearchBar
			{
				Placeholder = "ユーザーの名前等",
				SearchCommand = new Command
				(
					async () =>
					{
						List.IsVisible = false;
						Indicator.IsVisible = true;
						Indicator.IsRunning = true;
						List.ItemsSource = GitHub.SearchResult<GitHub.SearchUser>.Parse
						(
							await Domain.GetStringFromUrlAsync
							(
								GitHub.GetSearchUsersUrl(Search.Text)
							)
						).Items;
						Indicator.IsRunning = false;
						Indicator.IsVisible = false;
						List.IsVisible = true;
					}
				),
			};

			Indicator = new ActivityIndicator
			{
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				IsVisible = false,
			};

			Content = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				//Spacing = 1.0,
				//BackgroundColor = Color.Gray,
				Children =
				{
					Search,
					List,
					Indicator,
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


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

		public AlphaSelectUserPage(Action<string> aReciever)
		{
			Reciever = aReciever;
			Title = L["Select User"];

			var Template = new DataTemplate(typeof(AlphaCircleImageCell));
			Template.SetBinding(AlphaCircleImageCell.ImageSourceProperty, "AvatarUrl");
			Template.SetBinding(AlphaCircleImageCell.TextProperty, "Login");

			List = new ListView
			{
				ItemTemplate = Template,
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
					async () => List.ItemsSource = GitHub.SearchResult<GitHub.SearchUser>.Parse
					(
						await Domain.GetStringFromUrlAsync
						(
							GitHub.GetSearchUsersUrl(Search.Text)
						)
					)
					.Items
				),
			};

			Content = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Spacing = 1.0,
				BackgroundColor = Color.Gray,
				Children =
				{
					Search,
					List,
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


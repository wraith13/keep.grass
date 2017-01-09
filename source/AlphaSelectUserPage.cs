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

		public class ListItem
		{
			public ImageSource ImageSource { get; set; }
			public string Text { get; set; }

			public static ListItem Make(GitHub.SearchUser User)
			{
				return new ListItem
				{
					ImageSource = User.AvatarUrl,
					Text = User.Login,
				};
			}
			public static ListItem Make(string User)
			{
				return new ListItem
				{
					ImageSource = GitHub.GetIconUrl(User),
					Text = User,
				};
			}
		}

		public AlphaSelectUserPage(Action<string> aReciever)
		{
			Reciever = aReciever;
			Title = L["Select User"];

			List = new ListView
			{
				ItemTemplate = new DataTemplateEx(typeof(AlphaCircleImageCell))
					.SetBindingList("ImageSource", "Text"),
			};
			List.ItemTapped += (sender, e) =>
			{
				var User = e.Item as ListItem;
				if (null != User)
				{
					Debug.WriteLine($"Select User: {User.Text}");
					Reciever(User.Text);
					Domain.AddRecentUser(User.Text);
				}
				AlphaFactory.MakeSureApp().Navigation.PopAsync();
			};
			List.ItemsSource = Domain.GetRecentUsers()
				.Select(i => ListItem.Make(i));

			Search = new SearchBar
			{
				Placeholder = L["ユーザーの名前等"],
				SearchCommand = new Command
				(
					() =>
					{
						List.IsVisible = false;
						Indicator.IsVisible = true;
						Indicator.IsRunning = true;

						Domain.GetStringFromUrlAsync
						(
							GitHub.GetSearchUsersUrl(Search.Text)
						 ).ContinueWith
						(
							t =>
							{
								if (null == t.Exception)
								{
									Device.BeginInvokeOnMainThread
									(
										() =>
										{
											List.ItemsSource = GitHub.SearchResult<GitHub.SearchUser>.Parse
											(
												t.Result
											)
											.Items
											.Select(i => ListItem.Make(i));
											Indicator.IsRunning = false;
											Indicator.IsVisible = false;
											List.IsVisible = true;
										}
									);
								}
								else
								{
									Debug.WriteLine(t.Exception);
								}
							}
						);
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


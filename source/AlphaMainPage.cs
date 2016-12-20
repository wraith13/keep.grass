using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using keep.grass.Helpers;

namespace keep.grass
{
	public class AlphaMainPage : ResponsiveContentPage
	{
		Languages.AlphaLanguage L = AlphaFactory.MakeSureLanguage();
		AlphaDomain Domain = AlphaFactory.MakeSureDomain();

		AlphaUserCircleGraph CircleGraph = AlphaFactory.MakeUserCircleGraph();
		AlphaUserCircleGraph[] Friends;
		AlphaActivityIndicatorButton UpdateButton = AlphaFactory.MakeActivityIndicatorButton();

		Task UpdateLeftTimeTask = null;
		DateTime UpdateLeftTimeTaskLastStamp = default(DateTime);

		public AlphaMainPage()
		{
			Title = "keep.grass";

			UpdateButton.Command = new Command(async o => await Domain.ManualUpdateLastPublicActivityAsync());
			//Build();

			CircleGraph.BackgroundColor = Color.White;
			CircleGraph.IsVisibleLeftTimeBar = true;
			CircleGraph.IsVisibleSatelliteTexts = true;
			CircleGraph.IsDoughnut = true;
			CircleGraph.Now = DateTime.Now;
			if
			(
				!string.IsNullOrWhiteSpace(Settings.UserName) &&
				Settings.GetIsValidUserName(Settings.UserName)
			)
			{
				CircleGraph.LastPublicActivity = Domain.GetLastPublicActivity(Settings.UserName);
			}
			CircleGraph
				.GestureRecognizers
				.Add
				(
					new TapGestureRecognizer()
					{
						Command = new Command(o => AlphaFactory.MakeSureApp().ShowDetailPage(Settings.UserName)),
					}
				);
		}

		public override void Build()
		{
			base.Build();
			Debug.WriteLine("AlphaMainPage.Rebuild();");

			if (null == Friends || Settings.GetFriendCount() != Friends.Count())
			{
				//Friends = Settings.GetFriendList().Select(i => AlphaFactory.MakeCircleImageCell()).ToArray();
				Friends = Settings.GetFriendList().Select(i => AlphaFactory.MakeUserCircleGraph()).ToArray();
				for (var i = 0; i < Friends?.Count(); ++i)
				{
					var Friend = Settings.GetFriend(i);
					var FriendCircle = Friends[i];
					FriendCircle.BackgroundColor = Color.White;
					FriendCircle.IsVisibleLeftTimeBar = false;
					FriendCircle.IsVisibleSatelliteTexts = false;
					FriendCircle.FontSize *= 0.5f;
					FriendCircle.CircleMargin = new Thickness(2.0);
					FriendCircle.IsDoughnut = true;
					FriendCircle.User = Friend;
					FriendCircle.Now = DateTime.Now;
					if (Settings.GetIsValidUserName(Friend))
					{
						FriendCircle.LastPublicActivity = Domain.GetLastPublicActivity(Friend);
					}
					FriendCircle.GestureRecognizers.Clear();
					FriendCircle
						.GestureRecognizers
						.Add
						(
							new TapGestureRecognizer()
							{
								Command = new Command(o => AlphaFactory.MakeSureApp().ShowDetailPage(Friend)),
							}
						);
				}
			}

			UpdateButton.Text = L["Update"];
			var ButtonFrame = new Grid()
			{
				VerticalOptions = LayoutOptions.End,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
			}
			.HorizontalJustificate
			(
				UpdateButton,
				new Button
				{
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					Text = L["Settings"],
					Command = new Command(o => AlphaFactory.MakeSureApp().ShowSettingsPage()),
				}
			);
			ButtonFrame.BackgroundColor = Color.White;

			if (Width <= Height)
			{
				CircleGraph.WidthRequest = Width;
				CircleGraph.HeightRequest = Height * 0.60;
				CircleGraph.HorizontalOptions = LayoutOptions.FillAndExpand;
				CircleGraph.VerticalOptions = LayoutOptions.FillAndExpand;

				foreach(var Friend in Friends)
				{
					Friend.WidthRequest = Width / Friends.Count();
					Friend.HeightRequest = Friend.WidthRequest;
					Friend.HorizontalOptions = LayoutOptions.CenterAndExpand;
					Friend.VerticalOptions = LayoutOptions.CenterAndExpand;
				}

				Content = new StackLayout
				{
					Spacing = 1.0,
					BackgroundColor = Color.Gray,
					Children =
					{
						CircleGraph,
						//MainTable,
						new Grid()
						{
							VerticalOptions = LayoutOptions.Start,
							HorizontalOptions = LayoutOptions.CenterAndExpand,
							ColumnSpacing = 0.0,
							RowSpacing = 0.0,
						}.HorizontalJustificate(Friends),
						ButtonFrame,
					},
				};
			}
			else
			{
				CircleGraph.WidthRequest = Width * 0.55;
				CircleGraph.HeightRequest = Height * 0.70;
				CircleGraph.HorizontalOptions = LayoutOptions.FillAndExpand;
				CircleGraph.VerticalOptions = LayoutOptions.FillAndExpand;

				foreach (var Friend in Friends)
				{
					Friend.WidthRequest = CircleGraph.WidthRequest / CircleGraph.Phi;
					Friend.HeightRequest = Height / Friends.Count();
					Friend.HorizontalOptions = LayoutOptions.CenterAndExpand;
					Friend.VerticalOptions = LayoutOptions.CenterAndExpand;
				}

				Content = new StackLayout
				{
					Spacing = 1.0,
					BackgroundColor = Color.Gray,
					Children =
					{
						new StackLayout
						{
							Orientation = StackOrientation.Horizontal,
							Spacing = 0.0,
							Children =
							{
								CircleGraph,
								//MainTable,
								new Grid()
								{
									ColumnSpacing = 0.0,
									RowSpacing = 0.0,
								}.VerticalJustificate(Friends),
							},
						},
						ButtonFrame,
					},
				};
			}

			//	Indicator を表示中にレイアウトを変えてしまうと簡潔かつ正常に Indicator を再表示できないようなので、問答無用でテキストを表示してしまう。
			UpdateButton.ShowText();

			CircleGraph.IsInvalidCanvas = true;
			foreach (var Friend in Friends)
			{
				Friend.IsInvalidCanvas = true;
			}
			OnUpdateLastPublicActivity(Settings.UserName, Domain.GetLastPublicActivity(Settings.UserName));
		}

		protected override void OnAppearing()
		{
			Debug.WriteLine("AlphaMainPage::OnAppearing");
			base.OnAppearing();
			UpdateInfoAsync();
			StartUpdateLeftTimeTask();
		}

		protected override void OnDisappearing()
		{
			Debug.WriteLine("AlphaMainPage::OnDisappearing");
			base.OnDisappearing();
			OnPause();
			CircleGraph.IsActive = false;
			CircleGraph.ResetTime();
			foreach (var Friend in Friends)
			{
				Friend.IsActive = false;
				Friend.ResetTime();
			}
		}

		public void OnPause()
		{
			StopUpdateLeftTimeTask();
		}

		public void OnStartQuery()
		{
			UpdateButton.ShowIndicator();
		}
		public void OnUpdateLastPublicActivity(string User, DateTime LastPublicActivity)
		{
			if (Settings.UserName == User)
			{
				CircleGraph.LastPublicActivity = LastPublicActivity;
			}
			foreach (var Friend in Friends)
			{
				if (Friend.User == User)
				{
					Friend.LastPublicActivity = LastPublicActivity;
				}
			}
		}
		public void OnUpdateIcon(string User, byte[] Binary)
		{
			if (Settings.UserName == User)
			{
				CircleGraph.Image = Binary;
			}
			for (var i = 0; i < Friends?.Count(); ++i)
			{
				/*
				var FriendLable = Friends[i];
				if (FriendLable.Text == User && null == FriendLable.ImageSource)
				{
					FriendLable.ImageSource = ImageSource.FromStream(() => new System.IO.MemoryStream(Binary));
				}
				*/
				var FriendCircle = Friends[i];
				if (FriendCircle.User == User)
				{
					FriendCircle.Image = Binary;
				}
			}
		}
		public void OnErrorInQuery()
		{
		}
		public void OnEndQuery()
		{
			UpdateButton.ShowText();
			StartUpdateLeftTimeTask();
		}

		public void UpdateInfoAsync()
		{
			Debug.WriteLine("AlphaMainPage::UpdateInfoAsync");
			var User = Settings.UserName;
			if (CircleGraph.User != User)
			{
				if (!String.IsNullOrWhiteSpace(User))
				{
					CircleGraph.User = User;

					if (!Settings.GetIsValidUserName(User))
					{
						ClearActiveInfo();
					}
					else
					{
						OnUpdateLastPublicActivity(User, Domain.GetLastPublicActivity(User));
					}
					Task.Run(() => Domain.ManualUpdateLastPublicActivityAsync());
				}
				else
				{
					CircleGraph.User = null;
					ClearActiveInfo();
				}
			}

			for (var i = 0; i < Friends?.Count(); ++i)
			{
				var Friend = Settings.GetFriend(i);
				/*
				var FriendLable = Friends[i];
				if (FriendLable.Text != Friend)
				{
					FriendLable.ImageSource = null;
					var Binary = AlphaImageProxy.GetFromCache(GitHub.GetIconUrl(Friend));
					if (Binary?.Any() ?? false)
					{
						FriendLable.ImageSource = ImageSource.FromStream(() => new System.IO.MemoryStream(Binary));
					}
					//AlphaFactory.MakeImageSourceFromUrl(GitHub.GetIconUrl(Friend))
					//		.ContinueWith(t => Device.BeginInvokeOnMainThread(() => FriendLable.ImageSource = t.Result));
					FriendLable.Text = Friend;
					FriendLable.TextColor = Color.Default;
					FriendLable.Command = new Command(o => AlphaFactory.MakeSureApp().ShowDetailPage(Friend));
				}
				*/
				var FriendCircle = Friends[i];
				if (FriendCircle.User != Friend)
				{
					FriendCircle.User = Friend;
					FriendCircle.GestureRecognizers.Clear();
					FriendCircle
						.GestureRecognizers
						.Add
						(
							new TapGestureRecognizer()
							{
								Command = new Command(o => AlphaFactory.MakeSureApp().ShowDetailPage(Friend)),
							}
						);
				}
			}
		}
		public void ClearActiveInfo()
		{
			CircleGraph.LastPublicActivity = default(DateTime);
		}

		public void StartUpdateLeftTimeTask(bool IsPersistently = true)
		{
			Debug.WriteLine("AlphaMainPage::StartUpdateLeftTimeTask");
			if (null == UpdateLeftTimeTask || UpdateLeftTimeTaskLastStamp.AddMilliseconds(3000) < DateTime.Now)
			{
				Debug.WriteLine("AlphaMainPage::StartUpdateLeftTimeTask::kick!!!");
				UpdateLeftTimeTask = new Task
				(
					() =>
					{
						while (null != UpdateLeftTimeTask)
						{
							var Now = DateTime.Now;
							UpdateLeftTimeTaskLastStamp = Now;
							Device.BeginInvokeOnMainThread
							(
								() =>
								{
									CircleGraph.Now = Now;
									foreach (var Friend in Friends)
									{
										Friend.Now = Now;
									}
									Domain.AutoUpdateLastPublicActivityAsync();
								}
							);
							Task.Delay(1000 - DateTime.Now.Millisecond).Wait();
						}
					}
				);
				UpdateLeftTimeTask.Start();
			}
			else
			if (IsPersistently)
			{
				Task.Delay(TimeSpan.FromMilliseconds(5000)).ContinueWith
				(
					(t) =>
					{
						StartUpdateLeftTimeTask(false);
					}
			   );
			}
		}
		public void StopUpdateLeftTimeTask()
		{
			Debug.WriteLine("AlphaMainPage::StopUpdateLeftTimeTask");
			UpdateLeftTimeTask = null;
		}
	}
}

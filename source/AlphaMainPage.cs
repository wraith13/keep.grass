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

		AlphaCircleImageCell[] Friends;
		AlphaActivityIndicatorTextCell LastActivityStampLabel = AlphaFactory.MakeActivityIndicatorTextCell();
		AlphaActivityIndicatorTextCell LeftTimeLabel = AlphaFactory.MakeActivityIndicatorTextCell();
		AlphaUserCircleGraph CircleGraph = AlphaFactory.MakeUserCircleGraph();
		AlphaActivityIndicatorButton UpdateButton = AlphaFactory.MakeActivityIndicatorButton();

		Task UpdateLeftTimeTask = null;
		DateTime UpdateLeftTimeTaskLastStamp = default(DateTime);

		public AlphaMainPage()
		{
			Title = "keep.grass";

			LastActivityStampLabel.Command = new Command(async o => await Domain.ManualUpdateLastPublicActivityAsync());
			//LeftTimeLabel.Command = new Command(async o => await Domain.ManualUpdateLastPublicActivityAsync());
			UpdateButton.Command = new Command(async o => await Domain.ManualUpdateLastPublicActivityAsync());
			//Build();

			CircleGraph.IsDoughnut = true;
			CircleGraph.Now = DateTime.Now;
			if
			(
				!string.IsNullOrWhiteSpace(Settings.UserName) &&
				Settings.GetIsValidUserName(Settings.UserName) ?
			)
			{
				CircleGraph.LastPublicActivity = Domain.GetLastPublicActivity(Settings.UserName);
			}
		}

		public override void Build()
		{
			base.Build();
			Debug.WriteLine("AlphaMainPage.Rebuild();");

			CircleGraph.Build(Width, Height);

			if (null == Friends || Settings.GetFriendCount() != Friends.Count())
			{
				Friends = Settings.GetFriendList().Select(i => AlphaFactory.MakeCircleImageCell()).ToArray();
			}

			CircleGraph
				.AsView()
				.GestureRecognizers
				.Add
				(
					new TapGestureRecognizer()
					{
						Command = new Command(o => AlphaFactory.MakeSureApp().ShowDetailPage(Settings.UserName)),
					}
				);

			var MainTable = Friends.Any() ?
			new TableView
			{
				BackgroundColor = Color.White,
				Root = new TableRoot
				{
					new TableSection(L["Rivals"])
					{
						Friends,
					},
				},
			}:
			new TableView
			{
				BackgroundColor = Color.White,
				Root = new TableRoot
				{
					new TableSection(L["Last Acitivity Stamp"])
					{
						LastActivityStampLabel,
					},
					new TableSection(L["Left Time"])
					{
						LeftTimeLabel,
					},
				},
			};
			UpdateButton.Text = L["Update"];
			var ButtonFrame = new Grid().HorizontalJustificate
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
				Content = new StackLayout
				{
					Spacing = 1.0,
					BackgroundColor = Color.Gray,
					Children =
					{
						CircleGraph.AsView(),
						MainTable,
						ButtonFrame,
					},
				};
			}
			else
			{
				Content = new StackLayout
				{
					Spacing = 1.0,
					BackgroundColor = Color.Gray,
					Children =
					{
						new StackLayout
						{
							Orientation = StackOrientation.Horizontal,
							Spacing = 1.0,
							Children =
							{
								CircleGraph.AsView(),
								MainTable,
							},
						},
						ButtonFrame,
					},
				};
			}

			//	Indicator を表示中にレイアウトを変えてしまうと簡潔かつ正常に Indicator を再表示できないようなので、問答無用でテキストを表示してしまう。
			LastActivityStampLabel.ShowText();
			LeftTimeLabel.ShowText();
			UpdateButton.ShowText();

			OnUpdateLastPublicActivity(Settings.UserName, Domain.GetLastPublicActivity(Settings.UserName));
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			UpdateInfoAsync();
			StartUpdateLeftTimeTask();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			StopUpdateLeftTimeTask();
			CircleGraph.IsVisible = false;
		}

		public void OnStartQuery()
		{
			LastActivityStampLabel.ShowIndicator();
			LeftTimeLabel.ShowIndicator();
			UpdateButton.ShowIndicator();
		}
		public void OnUpdateLastPublicActivity(string User, DateTime LastPublicActivity)
		{
			if (Settings.UserName == User)
			{
				CircleGraph.LastPublicActivity = LastPublicActivity;
				LastActivityStampLabel.Text = LastPublicActivity.IsDefault() ?
					"" :
					LastPublicActivity.ToString("yyyy-MM-dd HH:mm:ss");
				LastActivityStampLabel.TextColor = Color.Default;
			}
		}
		public void OnErrorInQuery()
		{
			LastActivityStampLabel.Text = L["Error"];
			LastActivityStampLabel.TextColor = Color.Red;
		}
		public void OnEndQuery()
		{
			LastActivityStampLabel.ShowText();
			LeftTimeLabel.ShowText();
			UpdateButton.ShowText();
			StartUpdateLeftTimeTask();
		}

		public void UpdateInfoAsync()
		{
			Debug.WriteLine("AlphaMainPage::UpdateInfoAsync");
			var User = Settings.UserName;
			if (!String.IsNullOrWhiteSpace(User))
			{
				if (CircleGraph.AltText != User)
				{
					CircleGraph.User = User;

					if (!Settings.GetIsValidUserName(User))
					{
						ClearActiveInfo();
					}
					if (default(DateTime) == Domain.GetLastPublicActivity(User))
					{
						Task.Run(() => Domain.ManualUpdateLastPublicActivityAsync());
					}
				}
			}
			else
			{
				CircleGraph.Image = null;
				CircleGraph.AltText = L["unspecified"];
				CircleGraph.AltTextColor = Color.Gray;
				ClearActiveInfo();
			}

			for (var i = 0; i < Friends?.Count(); ++i)
			{
				var Friend = Settings.GetFriend(i);
				var FriendLable = Friends[i];
				if (FriendLable.Text != Friend)
				{
					FriendLable.ImageSource = null;
					AlphaFactory.MakeImageSourceFromUrl(GitHub.GetIconUrl(Friend))
							.ContinueWith(t => Device.BeginInvokeOnMainThread(() => FriendLable.ImageSource = t.Result));
					FriendLable.Text = Friend;
					FriendLable.TextColor = Color.Default;
					FriendLable.Command = new Command(o => AlphaFactory.MakeSureApp().ShowDetailPage(Friend));
				}
			}
		}
		public void ClearActiveInfo()
		{
			CircleGraph.LastPublicActivity = default(DateTime);
			LastActivityStampLabel.Text = "";
			LeftTimeLabel.Text = "";
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
							CircleGraph.Now = Now;
							Device.BeginInvokeOnMainThread(() => UpdateLeftTime(Now, Domain.GetLastPublicActivity(Settings.UserName)));
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

		protected void UpdateLeftTime(DateTime Now, DateTime LastPublicActivity)
		{
			if (default(DateTime) != LastPublicActivity)
			{
				var Today = Now.Date;
				var LimitTime = LastPublicActivity.AddHours(24);
				var LeftTime = LimitTime - Now;
				LeftTimeLabel.Text = Math.Floor(LeftTime.TotalHours).ToString() +LeftTime.ToString("\\:mm\\:ss");
				LeftTimeLabel.TextColor = AlphaDomain.MakeLeftTimeColor(LeftTime);

				Task.Run(() => Domain.AutoUpdateLastPublicActivityAsync());
			}
			else
			{
				LeftTimeLabel.Text = "";
				/*if (Settings.IsValidUserName)
				{
					StopUpdateLeftTimeTask();
				}*/
			}

			for (var i = 0; i < Friends?.Count(); ++i)
			{
				var Friend = Settings.GetFriend(i);
				var LimitTime = Domain.GetLastPublicActivity(Friend).AddHours(24);
				var LeftTime = LimitTime - Now;
				var FriendLable = Friends[i];
				FriendLable.TextColor = AlphaDomain.MakeLeftTimeColor(LeftTime);
			}

			//Debug.WriteLine("AlphaMainPage::UpdateLeftTime::LeftTime = " +LeftTimeLabel.Text);
		}

	}
}

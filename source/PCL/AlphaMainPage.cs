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
		IEnumerable<AlphaUserCircleGraph> CircleGraphList => new AlphaUserCircleGraph[] { CircleGraph }.Concat
		(
			Friends ?? new AlphaUserCircleGraph[] { }
		);
		public void ApplyCircleGraph(string User, Action<AlphaUserCircleGraph> Apply)
		{
			var Target = CircleGraphList.Where(i => i.User == User).FirstOrDefault();
			if (null != Target)
			{
				Apply(Target);
			}
		}
		public void ApplyCircleGraph(Action<AlphaUserCircleGraph> Apply)
		{
			foreach (var i in CircleGraphList)
			{
				Apply(i);
			}
		}
		AlphaActivityIndicatorButton UpdateButton = AlphaFactory.MakeActivityIndicatorButton();

		Task UpdateLeftTimeTask = null;
		DateTime UpdateLeftTimeTaskLastStamp = default(DateTime);

		public AlphaMainPage()
		{
			Title = "keep.grass";

			UpdateButton.Command = new Command(async o => await Domain.ManualUpdateLastPublicActivityAsync());

			InitCircleGraph(CircleGraph, Settings.UserName);
			CircleGraph.HorizontalOptions = LayoutOptions.FillAndExpand;
			CircleGraph.VerticalOptions = LayoutOptions.FillAndExpand;
			CircleGraph.IsVisibleLeftTimeBar = true;
			CircleGraph.IsVisibleSatelliteTexts = true;
		}
		public void InitCircleGraph(AlphaUserCircleGraph i, string User)
		{
			i.BackgroundColor = Color.White;
			i.IsDoughnut = true;
			i.Now = DateTime.Now;
			i.User = User;
			i.GestureRecognizers.Clear();
			i.LastPublicActivity = default(DateTime);
			if (!string.IsNullOrWhiteSpace(User))
			{
				if (Settings.GetIsValidUserName(User))
				{
					i.LastPublicActivity = Domain.GetLastPublicActivity(User);
				}
				i.GestureRecognizers
					.Add
					(
						new TapGestureRecognizer()
						{
							Command = new Command(o => AlphaFactory.MakeSureApp().ShowDetailPage(User)),
						}
					);
			}
		}

		public override void Build()
		{
			base.Build();
			Debug.WriteLine("AlphaMainPage.Rebuild();");

			if (null == Friends || Settings.GetFriendCount() != Friends.Count())
			{
				Friends = Settings.GetFriendList().Select(i => AlphaFactory.MakeUserCircleGraph()).ToArray();
				for (var i = 0; i < Friends?.Count(); ++i)
				{
					var Friend = Settings.GetFriend(i);
					var FriendCircle = Friends[i];
					InitCircleGraph(FriendCircle, Friend);
					FriendCircle.HorizontalOptions = LayoutOptions.Center;
					FriendCircle.VerticalOptions = LayoutOptions.Center;
					FriendCircle.IsVisibleLeftTimeBar = false;
					FriendCircle.IsVisibleSatelliteTexts = false;
					FriendCircle.FontSize *= 0.5f;
					FriendCircle.CircleMargin = new Thickness(2.0);
				}
			}

			UpdateButton.Text = L["Update"];
			var ButtonFrame = new Grid()
			{
				VerticalOptions = LayoutOptions.End,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = Color.White,
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

			if (Width <= Height)
			{
				CircleGraph.WidthRequest = Width;
				CircleGraph.HeightRequest = Math.Floor(Height * 0.10);
				foreach(var Friend in Friends)
				{
					Friend.WidthRequest = Math.Floor(Width / Math.Min(Math.Max(Friends.Count(), 2), 4));
					Friend.HeightRequest = Friend.WidthRequest;
				}
				var StackContent = new StackLayout
				{
					Spacing = 0.0,
					BackgroundColor = Color.White,
				};
				StackContent.Children.Add(CircleGraph);
				BuildFriends(StackContent, StackOrientation.Horizontal);
				StackContent.Children.Add(ButtonFrame);
				Content = StackContent;
			}
			else
			{
				CircleGraph.WidthRequest = Math.Floor(Width * 0.10);
				CircleGraph.HeightRequest = Math.Floor(Height * 0.60);
				foreach (var Friend in Friends)
				{
					Friend.HeightRequest = Math.Floor(Height / Math.Min(Math.Max(Friends.Count(), 2), 4));
					Friend.WidthRequest = Friend.HeightRequest;
				}
				var StackContent = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					VerticalOptions = LayoutOptions.FillAndExpand,
					Spacing = 0.0,
					BackgroundColor = Color.White,
				};
				StackContent.Children.Add(CircleGraph);
				BuildFriends(StackContent, StackOrientation.Vertical);
				Content = new StackLayout
				{
					Spacing = 0.0,
					BackgroundColor = Color.White,
					Children =
					{
						StackContent,
						ButtonFrame,
					},
				};
			}

			//	Indicator を表示中にレイアウトを変えてしまうと簡潔かつ正常に Indicator を再表示できないようなので、問答無用でテキストを表示してしまう。
			UpdateButton.ShowText();

			ApplyCircleGraph(i => i.IsInvalidCanvas = true);
			OnUpdateLastPublicActivity(Settings.UserName, Domain.GetLastPublicActivity(Settings.UserName));
		}

		public void BuildFriends(StackLayout Stack, StackOrientation Orientation)
		{
			var RowCount = 3.0;
			var LineCount = (int)Math.Ceiling(Friends.Count() / RowCount);
			var CirclePerLine = ((float)Friends.Count()) / (float)LineCount;
			Func<int, int> CalcIndex = (int i) => (int)Math.Ceiling((CirclePerLine * i) - ((1.0 / RowCount) + 0.1));
			for (var i = 0; i < LineCount; ++i)
			{
				var SkipCount = CalcIndex(i);
				var TakeCount = CalcIndex(i + 1) - SkipCount;
				var Adjuster = 0 == TakeCount % 2 ? 0.0 : 1.0;
				Stack.Children.Add
				(
					new Grid()
					{
						ColumnSpacing = Adjuster,
						RowSpacing = Adjuster,
					}.LineJustificate
					(
						Orientation,
						CirclePerLine <= (float)TakeCount ?
							GridEx.Justificate.Even:
							GridEx.Justificate.Odd,
						Friends
							.Skip(SkipCount)
							.Take(TakeCount)
							.ToArray()
					)
				);
			}
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
			ApplyCircleGraph
			(
				i =>
				{
					i.IsActive = false;
					i.ResetTime();
				}
			);
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
			ApplyCircleGraph(User, i => i.LastPublicActivity = LastPublicActivity);
		}
		public void OnUpdateIcon(string User, byte[] Binary)
		{
			ApplyCircleGraph(User, i => i.Image = Binary);
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
			var HasRequestToUpdate = false;
			var User = Settings.UserName;
			if (CircleGraph.User != User)
			{
				if (!String.IsNullOrWhiteSpace(User))
				{
					HasRequestToUpdate = true;
					InitCircleGraph(CircleGraph, User);
				}
				else
				{
					CircleGraph.User = null;
				}
			}

			for (var i = 0; i < Friends?.Count(); ++i)
			{
				var Friend = Settings.GetFriend(i);
				var FriendCircle = Friends[i];
				if (FriendCircle.User != Friend)
				{
					HasRequestToUpdate = true;
					InitCircleGraph(FriendCircle, Friend);
				}
			}
			if (HasRequestToUpdate)
			{
				Task.Run(() => Domain.ManualUpdateLastPublicActivityAsync());
			}
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
									ApplyCircleGraph(i => i.Now = Now);
                                    var dummy = Domain.AutoUpdateLastPublicActivityAsync();
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

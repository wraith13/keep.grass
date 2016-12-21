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
				CircleGraph.HeightRequest = Math.Floor(Height * 0.10);
				CircleGraph.HorizontalOptions = LayoutOptions.FillAndExpand;
				CircleGraph.VerticalOptions = LayoutOptions.FillAndExpand;
				foreach(var Friend in Friends)
				{
					Friend.WidthRequest = Math.Floor(Width / Math.Min(Math.Max(Friends.Count(), 2), 4));
					Friend.HeightRequest = Friend.WidthRequest;
					Friend.HorizontalOptions = LayoutOptions.Center;
					Friend.VerticalOptions = LayoutOptions.Center;
				}
				var StackContent = new StackLayout
				{
					Spacing = 0.0,
					BackgroundColor = Color.White,
				};
				StackContent.Children.Add(CircleGraph);
				var RowCount = 3.0;
				var LineCount = (int)Math.Ceiling(Friends.Count() / RowCount);
				var CirclePerLine = ((float)Friends.Count()) / (float)LineCount;
				Func<int,int> CalcIndex = (int i) => (int)Math.Ceiling((CirclePerLine * i) -((1.0 /RowCount) +0.1));
				for (var i = 0; i < LineCount; ++i)
				{
					var SkipCount = CalcIndex(i);
					var TakeCount = CalcIndex(i +1) -SkipCount;
					var Adjuster = 0 == TakeCount % 2 ? 0.0 : 1.0;
					StackContent.Children.Add
					(
						new Grid()
						{
							ColumnSpacing = Adjuster,
							RowSpacing = Adjuster,
						}.HorizontalJustificate
						(
							CirclePerLine <= (float)TakeCount ?
								GridUtil.Justificate.Even:
								GridUtil.Justificate.Odd,
							Friends
								.Skip(SkipCount)
								.Take(TakeCount)
								.ToArray()
						)
					);
				}
				StackContent.Children.Add(ButtonFrame);
				Content = StackContent;
			}
			else
			{
				CircleGraph.WidthRequest = Math.Floor(Width * 0.10);
				CircleGraph.HeightRequest = Math.Floor(Height * 0.60);
				CircleGraph.HorizontalOptions = LayoutOptions.FillAndExpand;
				CircleGraph.VerticalOptions = LayoutOptions.FillAndExpand;
				foreach (var Friend in Friends)
				{
					Friend.WidthRequest = Math.Floor(Height / Math.Min(Math.Max(Friends.Count(), 2), 4));
					Friend.WidthRequest = Friend.HeightRequest;
					Friend.HorizontalOptions = LayoutOptions.CenterAndExpand;
					Friend.VerticalOptions = LayoutOptions.Center;
				}
				var StackContent = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					VerticalOptions = LayoutOptions.FillAndExpand,
					Spacing = 0.0,
					BackgroundColor = Color.White,
				};
				StackContent.Children.Add(CircleGraph);
				var RowCount = 3.0;
				var LineCount = (int)Math.Ceiling(Friends.Count() / RowCount);
				var CirclePerLine = ((float)Friends.Count()) / (float)LineCount;
				Func<int, int> CalcIndex = (int i) => (int)Math.Ceiling((CirclePerLine * i) - ((1.0 /RowCount) +0.1));
				for (var i = 0; i < LineCount; ++i)
				{
					var SkipCount = CalcIndex(i);
					var TakeCount = CalcIndex(i + 1) - SkipCount;
					var Adjuster = 0 == TakeCount % 2 ? 0.0 : 1.0;
					StackContent.Children.Add
					(
						new Grid()
						{
							ColumnSpacing = Adjuster,
							RowSpacing = Adjuster,
						}.VerticalJustificate
						(
							CirclePerLine <= (float)TakeCount ?
								GridUtil.Justificate.Even :
								GridUtil.Justificate.Odd,
							Friends
								.Skip(SkipCount)
								.Take(TakeCount)
								.ToArray()
						)
					);
				}
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
			var IsChangedUser = false;
			var User = Settings.UserName;
			if (CircleGraph.User != User)
			{
				if (!String.IsNullOrWhiteSpace(User))
				{
					InitCircleGraph(CircleGraph, User);
					IsChangedUser = true;
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
					IsChangedUser = true;
					InitCircleGraph(FriendCircle, Friend);
				}
			}
			if (IsChangedUser)
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

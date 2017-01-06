using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using keep.grass.Helpers;

namespace keep.grass
{
	public class AlphaDetailPage : ResponsiveContentPage
	{
		AlphaApp Root = AlphaFactory.MakeSureApp();
		Languages.AlphaLanguage L = AlphaFactory.MakeSureLanguage();
		AlphaDomain Domain = AlphaFactory.MakeSureDomain();

		AlphaCircleImageCell UserLabel = AlphaFactory.MakeCircleImageCell();
		AlphaCircleImageCell LastActivityStampLabel = AlphaFactory.MakeCircleImageCell();
		AlphaActivityIndicatorTextCell LeftTimeLabel = AlphaFactory.MakeActivityIndicatorTextCell();
		AlphaUserCircleGraph CircleGraph = AlphaFactory.MakeUserCircleGraph();

		Task UpdateLeftTimeTask = null;
		DateTime UpdateLeftTimeTaskLastStamp = default(DateTime);

		public String User;

		public AlphaDetailPage(string UserName)
		{
			Title = User = UserName;

			LastActivityStampLabel.Command = new Command(o => AlphaFactory.MakeSureApp().ShowFeedPage(User));
			LeftTimeLabel.Command = new Command(async o => await Domain.ManualUpdateLastPublicActivityAsync());

			//Build();

			CircleGraph.BackgroundColor = Color.White;
			CircleGraph.IsVisibleSatelliteTexts = true;
			CircleGraph.IsDoughnut = false;
			CircleGraph.ActiveWait = TimeSpan.FromMilliseconds(100);
			CircleGraph.AnimationSpan = TimeSpan.FromMilliseconds(500);
			CircleGraph.Now = DateTime.Now;
		}

		public override void Build()
		{
			base.Build();
			Debug.WriteLine("AlphaDetailPage.Rebuild();");

			var MainTable = new TableView
			{
				BackgroundColor = Color.White,
				Root = new TableRoot
				{
					new TableSection(L["Github Account"])
					{
						UserLabel,
					},
					new TableSection(L["Last Activity Stamp"])
					{
						LastActivityStampLabel,
					},
					new TableSection(L["Left Time"])
					{
						LeftTimeLabel,
					},
				},
			};
			if (Width <= Height)
			{
				//CircleGraph.WidthRequest = Width;
				CircleGraph.HeightRequest = Height * 0.55;
				CircleGraph.HorizontalOptions = LayoutOptions.FillAndExpand;
				CircleGraph.VerticalOptions = LayoutOptions.Start;

				Content = new StackLayout
				{
					Spacing = 1.0,
					BackgroundColor = Color.Gray,
					Children =
					{
						CircleGraph,
						MainTable,
					},
				};
			}
			else
			{
				CircleGraph.WidthRequest = Width * 0.55;
				//CircleGraph.HeightRequest = Height;
				CircleGraph.HorizontalOptions = LayoutOptions.Start;
				CircleGraph.VerticalOptions = LayoutOptions.FillAndExpand;

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
								CircleGraph,
								MainTable,
							},
						},
					},
				};
			}

			//	Indicator を表示中にレイアウトを変えてしまうと簡潔かつ正常に Indicator を再表示できないようなので、問答無用でテキストを表示してしまう。
			//LastActivityStampLabel.ShowText();
			LeftTimeLabel.ShowText();

			CircleGraph.IsInvalidCanvas = true;
			OnUpdateLastPublicActivity();
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
		}

		public void OnStartQuery()
		{
			//LastActivityStampLabel.ShowIndicator();
			LeftTimeLabel.ShowIndicator();
		}
		public void OnUpdateLastPublicActivity()
		{
			var LastPublicActivity = Domain.GetLastPublicActivity(User);
			CircleGraph.LastPublicActivity = LastPublicActivity;
			LastActivityStampLabel.Text = Domain.ToString(LastPublicActivity);
			LastActivityStampLabel.TextColor = Color.Default;
		}
		public void OnErrorInQuery()
		{
			LastActivityStampLabel.Text = L["Error"];
			LastActivityStampLabel.TextColor = Color.Red;
		}
		public void OnEndQuery()
		{
			//LastActivityStampLabel.ShowText();
			LeftTimeLabel.ShowText();
			StartUpdateLeftTimeTask();
		}

		public void UpdateInfoAsync()
		{
			Debug.WriteLine("AlphaDetailPage::UpdateInfoAsync");
			if (!String.IsNullOrWhiteSpace(User))
			{
				if (UserLabel.Text != User)
				{
					AlphaFactory.MakeImageSourceFromUrl(GitHub.GetIconUrl(User))
						.ContinueWith(t => Device.BeginInvokeOnMainThread(() => UserLabel.ImageSource = t.Result));
					UserLabel.Text = User;
					UserLabel.TextColor = Color.Default;
					UserLabel.Command = new Command
					(
						o => Device.OpenUri
						(
							new Uri(GitHub.GetProfileUrl(User))
						)
					);
					UserLabel.OptionImageSource = Root.GetExportImageSource();
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
				UserLabel.ImageSource = null;
				UserLabel.Text = L["unspecified"];
				UserLabel.TextColor = Color.Gray;
				UserLabel.Command = null;
				UserLabel.OptionImageSource = null;
				ClearActiveInfo();
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
			Debug.WriteLine("AlphaDetailPage::StartUpdateLeftTimeTask");
			if (null == UpdateLeftTimeTask || UpdateLeftTimeTaskLastStamp.AddMilliseconds(3000) < DateTime.Now)
			{
				Debug.WriteLine("AlphaDetailPage::StartUpdateLeftTimeTask::kick!!!");
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
									UpdateLeftTime(Now, Domain.GetLastPublicActivity(User));
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
			Debug.WriteLine("AlphaDetailPage::StopUpdateLeftTimeTask");
			UpdateLeftTimeTask = null;
		}

		protected void UpdateLeftTime(DateTime Now, DateTime LastPublicActivity)
		{
			if (default(DateTime) != LastPublicActivity)
			{
				var Today = Now.Date;
				var LimitTime = LastPublicActivity.AddHours(24);
				var LeftTime = LimitTime - Now;
				LeftTimeLabel.Text = Domain.ToString(LeftTime);
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
			//Debug.WriteLine("AlphaDetailPage::UpdateLeftTime::LeftTime = " +LeftTimeLabel.Text);
		}
	}
}

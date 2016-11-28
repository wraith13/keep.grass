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
		AlphaActivityIndicatorTextCell LastActivityStampLabel = AlphaFactory.MakeActivityIndicatorTextCell();
		AlphaActivityIndicatorTextCell LeftTimeLabel = AlphaFactory.MakeActivityIndicatorTextCell();
		AlphaCircleGraph CircleGraph = AlphaFactory.MakeCircleGraph();

		Task UpdateLeftTimeTask = null;
		DateTime UpdateLeftTimeTaskLastStamp = default(DateTime);

		public String User;

		public AlphaDetailPage(string UserName)
		{
			Title = User = UserName;

			LastActivityStampLabel.Command = new Command(async o => await Domain.ManualUpdateLastPublicActivityAsync());
			//LeftTimeLabel.Command = new Command(async o => await Domain.ManualUpdateLastPublicActivityAsync());

			//Build();
		}

		public override void Build()
		{
			base.Build();
			Debug.WriteLine("AlphaDetailPage.Rebuild();");

			CircleGraph.Build(Width, Height);

			var MainTable = new TableView
			{
				BackgroundColor = Color.White,
				Root = new TableRoot
				{
					new TableSection(L["Github Account"])
					{
						UserLabel,
					},
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
					},
				};
			}

			//	Indicator を表示中にレイアウトを変えてしまうと簡潔かつ正常に Indicator を再表示できないようなので、問答無用でテキストを表示してしまう。
			LastActivityStampLabel.ShowText();
			LeftTimeLabel.ShowText();

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
			LastActivityStampLabel.ShowIndicator();
			LeftTimeLabel.ShowIndicator();
		}
		public void OnUpdateLastPublicActivity()
		{
			LastActivityStampLabel.Text = Domain.LastPublicActivity.IsDefault() ?
				"":
				Domain.LastPublicActivity.ToString("yyyy-MM-dd HH:mm:ss");
			LastActivityStampLabel.TextColor = Color.Default;
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
					if (!Settings.IsValidUserName)
					{
						ClearActiveInfo();
					}
					if (default(DateTime) == Domain.LastPublicActivity)
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
		public static float TimeToAngle(DateTime Time)
		{
			return (float)((Time.TimeOfDay.Ticks * 360) / TimeSpan.FromDays(1).Ticks);
		}
		public void ClearActiveInfo()
		{
			Domain.LastPublicActivity = default(DateTime);
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
							UpdateLeftTimeTaskLastStamp = DateTime.Now;
							Device.BeginInvokeOnMainThread(() => UpdateLeftTime());
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

		protected void UpdateLeftTime()
		{
			CircleGraph.SetStartAngle(TimeToAngle(DateTime.Now));
			if (default(DateTime) != Domain.LastPublicActivity)
			{
				var Now = DateTime.Now;
				var Today = Now.Date;
				var LimitTime = Domain.LastPublicActivity.AddHours(24);
				var LeftTime = LimitTime - Now;
				LeftTimeLabel.Text = Math.Floor(LeftTime.TotalHours).ToString() +LeftTime.ToString("\\:mm\\:ss");
				var LeftTimeColor = AlphaDomain.MakeLeftTimeColor(LeftTime);

				LeftTimeLabel.TextColor = LeftTimeColor;

				CircleGraph.SetStartAngle(TimeToAngle(Now));
				CircleGraph.Data = AlphaDomain.MakeSlices(LeftTime, LeftTimeColor);
				CircleGraph.SatelliteTexts = AlphaDomain.MakeSatelliteTexts(Now, Domain.LastPublicActivity);

				Task.Run(() => Domain.AutoUpdateLastPublicActivityAsync());
			}
			else
			{
				LeftTimeLabel.Text = "";
				/*if (Settings.IsValidUserName)
				{
					StopUpdateLeftTimeTask();
				}*/

				CircleGraph.Data = AlphaDomain.MakeSlices(TimeSpan.Zero, Color.Lime);
				CircleGraph.SatelliteTexts = Enumerable.Range(0, 24).Select
				(
					i => new CircleGraphSatelliteText
					{
						Text = i.ToString(),
						Color = Color.Gray,
						Angle = 360.0f * ((float)(i) / 24.0f),
					}
				);
			}
			CircleGraph.Update();
			//Debug.WriteLine("AlphaDetailPage::UpdateLeftTime::LeftTime = " +LeftTimeLabel.Text);
		}
	}
}

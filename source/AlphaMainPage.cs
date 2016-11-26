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

		AlphaCircleImageCell UserLabel = AlphaFactory.MakeCircleImageCell();
		AlphaCircleImageCell[] Friends;
		AlphaActivityIndicatorTextCell LastActivityStampLabel = AlphaFactory.MakeActivityIndicatorTextCell();
		AlphaActivityIndicatorTextCell LeftTimeLabel = AlphaFactory.MakeActivityIndicatorTextCell();
		AlphaCircleGraph CircleGraph = AlphaFactory.MakeCircleGraph();

		Task UpdateLeftTimeTask = null;
		DateTime UpdateLeftTimeTaskLastStamp = default(DateTime);

		public AlphaMainPage()
		{
			Title = "keep.grass";

			LastActivityStampLabel.Command = new Command(async o => await Domain.ManualUpdateLastPublicActivityAsync());
			//LeftTimeLabel.Command = new Command(async o => await Domain.ManualUpdateLastPublicActivityAsync());
			//Build();
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

			UserLabel.Command = Friends.Any() ?
				new Command(o => AlphaFactory.MakeSureApp().ShowDetailPage(Settings.UserName)):
				new Command(o => AlphaFactory.MakeSureApp().ShowSettingsPage());


			var MainTable = Friends.Any() ?
			new TableView
			{
				BackgroundColor = Color.White,
				Root = new TableRoot
				{
					new TableSection(L["Github Account"])
					{
						UserLabel,
					},
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
			var ButtonFrame = new Grid().HorizontalJustificate
			(
				new Button
				{
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					Text = L["Update"],
					Command = new Command(async o => await Domain.ManualUpdateLastPublicActivityAsync()),
				},
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
			Debug.WriteLine("AlphaMainPage::UpdateInfoAsync");
			var User = Settings.UserName;
			if (!String.IsNullOrWhiteSpace(User))
			{
				if (UserLabel.Text != User)
				{
					AlphaFactory.MakeImageSourceFromUrl(GitHub.GetIconUrl(User))
						.ContinueWith
			            (
		            		t => Device.BeginInvokeOnMainThread
				            (
		            			() =>
								{
									CircleGraph.Image = AlphaImageProxy.GetFromCache(GitHub.GetIconUrl(User));
									UserLabel.ImageSource = t.Result;
								}
				           )
			           	);
					UserLabel.Text = User;
					UserLabel.TextColor = Color.Default;
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
				CircleGraph.Image = null;
				UserLabel.ImageSource = null;
				UserLabel.Text = L["unspecified"];
				UserLabel.TextColor = Color.Gray;
				ClearActiveInfo();
			}

			for (var i = 0; i < Friends?.Count(); ++i)
			{
				var Friend = Settings.GetFriend(i);
				var FriendLable = Friends[i];
				if (FriendLable.Text != Friend)
				{
					AlphaFactory.MakeImageSourceFromUrl(GitHub.GetIconUrl(Friend))
							.ContinueWith(t => Device.BeginInvokeOnMainThread(() => FriendLable.ImageSource = t.Result));
					FriendLable.Text = Friend;
					FriendLable.TextColor = Color.Default;
					FriendLable.Command = new Command(o => AlphaFactory.MakeSureApp().ShowDetailPage(Friend));
				}
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
		public IEnumerable<TimePie> MakeSlices(TimeSpan LeftTime, Color LeftTimeColor)
		{
			if (0 <= LeftTime.Ticks)
			{
				//	※調整しておなかいと、表示上、経過時間と残り時間の合計が24時間より1秒足りない状態になってしまうので。
				var JustifiedLeftTime = new TimeSpan(LeftTime.Days, +LeftTime.Hours, LeftTime.Minutes, LeftTime.Seconds);
				var JustifiedElapsedTime = TimeSpan.FromDays(1) - JustifiedLeftTime;

				return new[]
				{
					new TimePie
					{
						Text = L["Left Time"],
						Value = JustifiedLeftTime,
						Color = LeftTimeColor,
					},
					new TimePie
					{
						Text = L["Elapsed Time"],
						Value = JustifiedElapsedTime,
						Color = Color.FromRgb(0xAA, 0xAA, 0xAA),
					},
				};
			}
			else
			{
				return new[]
				{
					new TimePie
					{
						Text = L["Left Time"],
						Value = TimeSpan.FromTicks(0),
						Color = Color.FromRgb(0xD6, 0xE6, 0x85),
					},
					new TimePie
					{
						Text = L["Elapsed Time"],
						Value = TimeSpan.FromDays(1),
						Color = Color.FromRgb(0xEE, 0x11, 0x11),
					},
				};
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
			Debug.WriteLine("AlphaMainPage::StopUpdateLeftTimeTask");
			UpdateLeftTimeTask = null;
		}

		public Color MakeLeftTimeColor(TimeSpan LeftTime)
		{
			double LeftTimeRate = Math.Max(0.0, Math.Min(1.0, LeftTime.TotalHours / 24.0));
			byte red = (byte)(255.0 * (1.0 - LeftTimeRate));
			byte green = (byte)(255.0 * Math.Min(0.5, LeftTimeRate));
			byte blue = 0;
			return  Color.FromRgb(red, green, blue);
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
				var LeftTimeColor = MakeLeftTimeColor(LeftTime);

				LeftTimeLabel.TextColor = LeftTimeColor;

				CircleGraph.SetStartAngle(TimeToAngle(Now));
				CircleGraph.Data = MakeSlices(LeftTime, LeftTimeColor);

				if (Friends?.Any() ?? false)
				{
					CircleGraph.SatelliteTexts = null;
				}
				else
				{
					CircleGraph.SatelliteTexts = Enumerable.Range(0, 24).Select
					(
						i => new
						{
							Hour = i,
							Time = Today + TimeSpan.FromHours(i),
						}
					)
					.Select
					(
						i => new
						{
							Hour = i.Hour,
							Time = i.Time.Ticks < Domain.LastPublicActivity.Ticks ?
									i.Time + TimeSpan.FromDays(1) :
									(
										TimeSpan.FromDays(1).Ticks < (i.Time - Domain.LastPublicActivity).Ticks ?
										i.Time - TimeSpan.FromDays(1) :
										i.Time
									),
						}
					)
					.Select
					(
						i => new CircleGraphSatelliteText
						{
							Text = i.Hour.ToString(),
							Color = LeftTime.Ticks <= 0 ?
								MakeLeftTimeColor(LeftTime) :
								i.Time.Ticks < Now.Ticks ?
									 Color.Gray :
									MakeLeftTimeColor(LimitTime - i.Time),
							Angle = 360.0f * ((float)(i.Hour) / 24.0f),
						}
					);
				}

				Task.Run(() => Domain.AutoUpdateLastPublicActivityAsync());
			}
			else
			{
				LeftTimeLabel.Text = "";
				/*if (Settings.IsValidUserName)
				{
					StopUpdateLeftTimeTask();
				}*/

				CircleGraph.Data = MakeSlices(TimeSpan.Zero, Color.Lime);
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
			//Debug.WriteLine("AlphaMainPage::UpdateLeftTime::LeftTime = " +LeftTimeLabel.Text);
		}
	}
}

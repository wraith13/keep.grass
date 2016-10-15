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
		AlphaActivityIndicatorTextCell LastActivityStampLabel = AlphaFactory.MakeActivityIndicatorTextCell();
		AlphaActivityIndicatorTextCell LeftTimeLabel = AlphaFactory.MakeActivityIndicatorTextCell();
#if WITH_PROGRESSBAR
		ProgressBar ProgressBar = new ProgressBar();
#endif
		VoidCircleGraph CircleGraph = AlphaFactory.MakeCircleGraph();

		Task UpdateLeftTimeTask = null;

		public AlphaMainPage()
		{
			Title = "keep.grass";

			UserLabel.Command = new Command(o => AlphaFactory.MakeSureApp().ShowSettingsPage());
			LastActivityStampLabel.Command = new Command(async o => await Domain.ManualUpdateLastPublicActivityAsync());
			//LeftTimeLabel.Command = new Command(async o => await Domain.ManualUpdateLastPublicActivityAsync());
#if WITH_PROGRESSBAR
			ProgressBar.Margin = new Thickness(0, 0, 0, 0);
#endif

			//Build();
		}

		public override void Build()
		{
			base.Build();
			Debug.WriteLine("AlphaMainPage.Rebuild();");

			CircleGraph.Build(Width, Height);

			var MainTable = new TableView
			{
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
#if WITH_PROGRESSBAR
			ProgressBarFrame,
			var ProgressBarFrame = new Grid().HorizontalJustificate
			(
				ProgressBar
		 	);
#endif
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
					Spacing = 0.5,
					BackgroundColor = Color.Gray,
					Children =
					{
						CircleGraph.AsView(),
						MainTable,
#if WITH_PROGRESSBAR
						ProgressBarFrame,
#endif
						ButtonFrame,
					},
				};
			}
			else
			{
				Content = new StackLayout
				{
					Spacing = 0.5,
					BackgroundColor = Color.Gray,
					Children =
					{
						new StackLayout
						{
							Orientation = StackOrientation.Horizontal,
							Spacing = 0.5,
							Children =
							{
								CircleGraph.AsView(),
								MainTable,
							},
						},
#if WITH_PROGRESSBAR
						ProgressBarFrame,
#endif
						ButtonFrame,
					},
				};
			}
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			UpdateInfoAsync().Wait(0);
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
			LastActivityStampLabel.Text = Domain.LastPublicActivity.ToString("yyyy-MM-dd HH:mm:ss");
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

		public async Task UpdateInfoAsync()
		{
			Debug.WriteLine("AlphaMainPage::UpdateInfoAsync");
			var User = Settings.UserName;
			if (!String.IsNullOrWhiteSpace(User))
			{
				if (UserLabel.Text != User)
				{
					UserLabel.ImageSource = GitHub.GetIconUrl(User);
					UserLabel.Text = User;
					UserLabel.TextColor = Color.Default;
					ClearActiveInfo();
					await Domain.ManualUpdateLastPublicActivityAsync();
				}
				else
				{
					StartUpdateLeftTimeTask();
				}
			}
			else
			{
				UserLabel.ImageSource = null;
				UserLabel.Text = L["unspecified"];
				UserLabel.TextColor = Color.Gray;
				ClearActiveInfo();
			}
		}
		public void ClearActiveInfo()
		{
			Domain.LastPublicActivity = default(DateTime);
			LastActivityStampLabel.Text = "";
			LeftTimeLabel.Text = "";

			CircleGraph.Data = MakeSlices(TimeSpan.Zero, Color.Lime);
		}
		public IEnumerable<TimePie> MakeSlices(TimeSpan LeftTime, Color LeftTimeColor)
		{
			if (0 <= LeftTime.Ticks)
			{
				return new[]
				{
					new TimePie
					{
						Text = L["Elapsed Time"],
						Value = TimeSpan.FromDays(1) -LeftTime,
						Color = Color.FromRgb(0xCC, 0xCC, 0xCC),
					},
					new TimePie
					{
						Text = L["Left Time"],
						Value = LeftTime,
						Color = LeftTimeColor,
					},
				};
			}
			else
			{
				return new[]
				{
					new TimePie
					{
						Text = L["Elapsed Time"],
						Value = TimeSpan.FromDays(1),
						Color = Color.FromRgb(0xEE, 0x11, 0x11),
					},
					new TimePie
					{
						Text = L["Left Time"],
						Value = TimeSpan.FromTicks(0),
						Color = Color.FromRgb(0xD6, 0xE6, 0x85),
					},
				};
			}
		}

		public void StartUpdateLeftTimeTask()
		{
			if (null == UpdateLeftTimeTask)
			{
				UpdateLeftTimeTask = new Task
				(
					() =>
					{
						while (null != UpdateLeftTimeTask)
						{
							Device.BeginInvokeOnMainThread(() => UpdateLeftTime());
							Task.Delay(1000 -DateTime.Now.Millisecond).Wait();
						}
					}
				);
				UpdateLeftTimeTask.Start();
			}
		}
		public void StopUpdateLeftTimeTask()
		{
			UpdateLeftTimeTask = null;
		}

#if WITH_PROGRESSBAR
		protected async void UpdateLeftTime()
#else
		protected void UpdateLeftTime()
#endif
		{
			if (default(DateTime) != Domain.LastPublicActivity)
			{
				var LeftTime = Domain.LastPublicActivity.AddHours(24) - DateTime.Now;
				LeftTimeLabel.Text = Math.Floor(LeftTime.TotalHours).ToString() +LeftTime.ToString("\\:mm\\:ss");
#if WITH_PROGRESSBAR
				await ProgressBar.ProgressTo(Math.Max(LeftTime.TotalDays, 0.0), 300, Easing.CubicInOut);
#endif

				double LeftTimeRate = Math.Max(0.0, Math.Min(1.0, LeftTime.TotalHours /24.0));
				byte red = (byte)(255.0 * (1.0 - LeftTimeRate));
				byte green = (byte)(255.0 *Math.Min(0.5, LeftTimeRate));
				byte blue = 0;
				var LeftTimeColor = Color.FromRgb(red, green, blue);

				LeftTimeLabel.TextColor = LeftTimeColor;

				CircleGraph.Data = MakeSlices(LeftTime, LeftTimeColor);
			}
			else
			{
				LeftTimeLabel.Text = "";
				if (Settings.IsValidUserName)
				{
					StopUpdateLeftTimeTask();
				}
			}
			//Debug.WriteLine("AlphaMainPage::UpdateLeftTime::LeftTime = " +LeftTimeLabel.Text);
		}
	}
}

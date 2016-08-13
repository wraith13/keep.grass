using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using keep.grass.Helpers;

namespace keep.grass
{
	public class AlphaMainPage : ContentPage
	{
		AlphaApp Root;
		public Languages.AlphaLanguage L;

		AlphaCircleImageCell UserLabel = AlphaFactory.MakeCircleImageCell();
		AlphaActivityIndicatorTextCell LastActivityStampLabel = AlphaFactory.MakeActivityIndicatorTextCell();
		AlphaActivityIndicatorTextCell LeftTimeLabel = AlphaFactory.MakeActivityIndicatorTextCell();
		ProgressBar ProgressBar = new ProgressBar();
		public DateTime ? LastPublicActivity;
		DateTime LastCheckStamp = default(DateTime);
		TimeSpan NextCheckTimeSpan = default(TimeSpan);

		Task UpdateLeftTimeTask = null;

		public AlphaMainPage(AlphaApp AppRoot)
		{
			Root = AppRoot;
			L = Root.L;
			Title = "keep.grass";

			UserLabel.Command = new Command
			(
				o =>
				{
					if (!String.IsNullOrWhiteSpace(UserLabel.Text))
					{
						Device.OpenUri(new Uri(GitHub.GetProfileUrl(UserLabel.Text)));
					}
				}
			);
			LastActivityStampLabel.Command = new Command
			(
				o =>
				{
					if (!String.IsNullOrWhiteSpace(UserLabel.Text))
					{
						Device.OpenUri(new Uri(GitHub.GetAcitivityUrl(UserLabel.Text)));
					}
				}
			);
			LeftTimeLabel.Command = new Command(o => ManualUpdateLastPublicActivityAsync().Wait(0));
			ProgressBar.Margin = new Thickness(0, 0, 0, 0);

			Rebuild();
		}

		public void Rebuild()
		{
			Content = new StackLayout
			{
				Children =
				{
					new TableView
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
					},
					new Grid().HorizontalJustificate
					(
						ProgressBar
					),
					new Grid().HorizontalJustificate
					(
						new Button
						{
							VerticalOptions = LayoutOptions.Center,
							HorizontalOptions = LayoutOptions.FillAndExpand,
							Text = L["Update"],
							Command = new Command(o => ManualUpdateLastPublicActivityAsync().Wait(0)),
						},
						new Button
						{
							VerticalOptions = LayoutOptions.Center,
							HorizontalOptions = LayoutOptions.FillAndExpand,
							Text = L["Settings"],
							Command = new Command(o => Root.ShowSettingsPage()),
						}
					)
				},
			};
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
					await ManualUpdateLastPublicActivityAsync();
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
			LastPublicActivity = null;
			LastActivityStampLabel.Text = "";
			LeftTimeLabel.Text = "";
		}

		public async Task AutoUpdateLastPublicActivityAsync()
		{
			Debug.WriteLine("AlphaMainPage::AutoUpdateInfoAsync");
			if (TimeSpan.FromSeconds(60) < DateTime.Now - LastCheckStamp)
			{
				await UpdateLastPublicActivityAsync();
			}
		}
		public async Task ManualUpdateLastPublicActivityAsync()
		{
			Debug.WriteLine("AlphaMainPage::ManualUpdateInfoAsync");
			await UpdateLastPublicActivityAsync();
			NextCheckTimeSpan = TimeSpan.FromSeconds(60);
		}
		private async Task UpdateLastPublicActivityAsync()
		{
			Debug.WriteLine("AlphaMainPage::UpdateLastPublicActivityAsync");
			var User = Settings.UserName;
			if (!String.IsNullOrWhiteSpace(User))
			{
				try
				{
					LastCheckStamp = DateTime.Now;
					LastActivityStampLabel.ShowIndicator();
					LeftTimeLabel.ShowIndicator();
					LastPublicActivity = await GitHub.GetLastPublicActivityAsync(User);
					var NewStamp = LastPublicActivity.Value.ToString("yyyy-MM-dd HH:mm:ss");
					Debug.WriteLine("AlphaMainPage::UpdateLastPublicActivityAsync::NewStamp = " +NewStamp);
					if (LastActivityStampLabel.Text != NewStamp)
					{
						LastActivityStampLabel.Text = NewStamp;
						LastActivityStampLabel.TextColor = Color.Default;
						Root.UpdateAlerts();
					}
					Settings.IsValidUserName = true;
				}
				catch(Exception err)
				{
					Debug.WriteLine("AlphaMainPage::UpdateLastPublicActivityAsync::catch::err" +err.ToString());
					//LastPublicActivity = null;
                    LastActivityStampLabel.Text = L["Error"];
                    LastActivityStampLabel.TextColor = Color.Red;
				}
				LastActivityStampLabel.ShowText();
				LeftTimeLabel.ShowText();
				StartUpdateLeftTimeTask();
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
							if (default(TimeSpan) < NextCheckTimeSpan && LastCheckStamp +NextCheckTimeSpan <= DateTime.Now)
							{
								NextCheckTimeSpan = TimeSpan.FromTicks
                            	(
	                                Math.Min
	                                (
		                                (NextCheckTimeSpan +TimeSpan.FromMinutes(1)).Ticks,
		                                TimeSpan.FromMinutes(20).Ticks
	                               	)
                               	);
								Device.BeginInvokeOnMainThread(async () => await AutoUpdateLastPublicActivityAsync());
							}
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

		protected async void UpdateLeftTime()
		{
			if (null != LastPublicActivity)
			{
				var LeftTime = LastPublicActivity.Value.AddHours(24) - DateTime.Now;
				LeftTimeLabel.Text = Math.Floor(LeftTime.TotalHours).ToString() +LeftTime.ToString("\\:mm\\:ss");
				await ProgressBar.ProgressTo(Math.Max(LeftTime.TotalDays, 0.0), 300, Easing.CubicInOut);
				if (LeftTime < TimeSpan.FromHours(0))
				{
					LeftTimeLabel.TextColor = Color.Red;
				}
				else
				if (LeftTime < TimeSpan.FromHours(3))
				{
					LeftTimeLabel.TextColor = Color.FromHex("FF8000");
				}
				else
				if (LeftTime < TimeSpan.FromHours(6))
				{
					LeftTimeLabel.TextColor = Color.FromHex("808000");
				}
				else
				if (LeftTime < TimeSpan.FromHours(12))
				{
					LeftTimeLabel.TextColor = Color.Blue;
				}
				else
				{
					LeftTimeLabel.TextColor = Color.Green;
				}
			}
			else
			{
				LeftTimeLabel.Text = "";
				if (Settings.IsValidUserName)
				{
					NextCheckTimeSpan = TimeSpan.FromSeconds(60);
				}
				else
				{
					StopUpdateLeftTimeTask();
				}
			}
			//Debug.WriteLine("AlphaMainPage::UpdateLeftTime::LeftTime = " +LeftTimeLabel.Text);
		}
	}
}

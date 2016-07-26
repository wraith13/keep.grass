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
		public DateTime ? LastPublicActivity;
		DateTime LastCheckStamp = default(DateTime);
		TimeSpan NextCheckTimeSpan = default(TimeSpan);

		Task UpdateLeftTimeTask = null;

		public AlphaMainPage(AlphaApp AppRoot)
		{
			Root = AppRoot;
			L = Root.L;
			Title = "keep.grass";

			UserLabel.Command = new Command(o => Root.ShowSettingsPage());
			LastActivityStampLabel.Command = new Command(o => ManualUpdateLastPublicActivityAsync().Wait(0));

			Rebuild();
		}

		public void Rebuild()
		{
			var ButtonGrid = new Grid
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Center,
				RowDefinitions =
				{
					new RowDefinition() { Height = GridLength.Auto }
				},
				ColumnDefinitions =
				{
					new ColumnDefinition
					{
						Width = new GridLength(1, GridUnitType.Star),
					},
					new ColumnDefinition
					{
						Width = new GridLength(1, GridUnitType.Star),
					},
				},
			};
			ButtonGrid.Children.Add
			(
				new Button
				{
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					Text = L["Update"],
					Command = LastActivityStampLabel.Command,
				},
				0, 0
			);
			ButtonGrid.Children.Add
			(
				new Button
				{
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					Text = L["Settings"],
					Command = UserLabel.Command,
				},
				1, 0
			);

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
					ButtonGrid
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
				}
				catch(Exception err)
				{
					Debug.WriteLine("AlphaMainPage::UpdateLastPublicActivityAsync::catch::err" +err.ToString());
					LastPublicActivity = null;
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

		protected void UpdateLeftTime()
		{
			if (null != LastPublicActivity)
			{
				var LeftTime = LastPublicActivity.Value.AddHours(24) - DateTime.Now;
				LeftTimeLabel.Text = Math.Floor(LeftTime.TotalHours).ToString() +LeftTime.ToString("\\:mm\\:ss");
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
					LeftTimeLabel.TextColor = Color.Aqua;
				}
				else
				{
					LeftTimeLabel.TextColor = Color.Green;
				}
			}
			else
			{
				LeftTimeLabel.Text = "";
				StopUpdateLeftTimeTask();
			}
			//Debug.WriteLine("AlphaMainPage::UpdateLeftTime::LeftTime = " +LeftTimeLabel.Text);
		}
	}
}

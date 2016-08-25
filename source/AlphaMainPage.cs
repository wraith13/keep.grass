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
			LeftTimeLabel.Command = new Command(async o => await Root.ManualUpdateLastPublicActivityAsync());
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
							Command = new Command(async o => await Root.ManualUpdateLastPublicActivityAsync()),
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
					await Root.ManualUpdateLastPublicActivityAsync();
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
			Root.LastPublicActivity = null;
			LastActivityStampLabel.Text = "";
			LeftTimeLabel.Text = "";
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

		protected async void UpdateLeftTime()
		{
			if (null != Root.LastPublicActivity)
			{
				var LeftTime = Root.LastPublicActivity.Value.AddHours(24) - DateTime.Now;
				LeftTimeLabel.Text = Math.Floor(LeftTime.TotalHours).ToString() +LeftTime.ToString("\\:mm\\:ss");
				await ProgressBar.ProgressTo(Math.Max(LeftTime.TotalDays, 0.0), 300, Easing.CubicInOut);

				double LeftTimeRate = Math.Max(0.0, Math.Min(1.0, LeftTime.TotalHours /24.0));
				byte red = (byte)(255.0 * (1.0 - LeftTimeRate));
				byte green = (byte)(255.0 *Math.Min(0.5, LeftTimeRate));
				byte blue = 0;
				
				LeftTimeLabel.TextColor = Color.FromRgb(red, green, blue);
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

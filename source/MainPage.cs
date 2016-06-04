using System;

using Xamarin.Forms;
using System.Threading.Tasks;

using keep.grass.Helpers;

namespace keep.grass
{
	public class MainPage : ContentPage
	{
		ImageCell UserLabel = new ImageCell();
		TextCell LastActivityStampLabel = new TextCell();
		TextCell LeftTimeLabel = new TextCell();
		DateTime LastPublicActivity;

		Task UpdateLeftTimeTask = null;

		public MainPage(App app)
		{
			Title = "keep.grass";

			var Command = new Command(o => app.navigation.PushAsync(new SettingsPage()));
			UserLabel.Command = Command;

			Content = new StackLayout { 
				Children =
				{
					new TableView
					{
						Root = new TableRoot
						{
							new TableSection("Github Account")
							{
								UserLabel,
							},
							new TableSection("Last Acitivity Stamp")
							{
								LastActivityStampLabel,
							},
							new TableSection("Left Time")
							{
								LeftTimeLabel,
							},
						}
					},
					new Button
					{
						Text = "Settings",
						Command = Command,
					},
				},
			};
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			UpdateInfoAsync().Wait(0);
		}

		protected async Task UpdateInfoAsync()
		{
			var User = Settings.UserName;
			if (!String.IsNullOrWhiteSpace(User))
			{
				UserLabel.ImageSource = GitHub.GetIconUrl(User);
				UserLabel.Text = User;
				UserLabel.TextColor = Color.Default;
			
				LastPublicActivity = await GitHub.GetLastPublicActivityAsync(User);
				LastActivityStampLabel.Text = LastPublicActivity.ToString("yyyy-MM-dd HH:mm:ss");

				if (null == UpdateLeftTimeTask)
				{
					UpdateLeftTimeTask = new Task
					(
						() =>
						{
							while(true)
							{
								Xamarin.Forms.Device.BeginInvokeOnMainThread(() => UpdateLeftTime());
								Task.Delay(100).Wait();
							}
						}
					);
					UpdateLeftTimeTask.Start();
				}
			}
			else
			{
				UserLabel.ImageSource = null;
				UserLabel.Text = "unspecified";
				UserLabel.TextColor = Color.Red;
				LastPublicActivity = default(DateTime);
				LastActivityStampLabel.Text = "";
				LeftTimeLabel.Text = "";
			}
		}

		protected void UpdateLeftTime()
		{
			if (default(DateTime) != LastPublicActivity)
			{
				var LeftTime = LastPublicActivity.AddHours(24) - DateTime.Now;
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
			}
		}
	}
}



using System;

using Xamarin.Forms;

using keep.grass.Helpers;

namespace keep.grass
{
	public class App : Application
	{
		NavigationPage navigation;
		Label UserLabel = new Label();
		Label LastActivityStampLabel = new Label();
		Label LeftTimeLabel = new Label();

		public App()
		{
			// The root page of your application
			MainPage = navigation = new NavigationPage
			(
				new ContentPage {
					Content = new StackLayout {
						VerticalOptions = LayoutOptions.Center,
						Children = {
							UserLabel,
							LastActivityStampLabel,
							LeftTimeLabel,
							new Button {
								Text = "Settings",
								Command = new Command(o => navigation.PushAsync(new SettingsPage())),
							},
						}
					}
				}
			);
			UpdateInfo();
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}
		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}
		protected override void OnResume()
		{
			// Handle when your app resumes
			UpdateInfo();
		}

		protected void UpdateInfo()
		{
			var User = Settings.UserName;
			if (!String.IsNullOrWhiteSpace(User))
			{
				UserLabel.Text = "User: " + User;
				UserLabel.TextColor = Color.Default;

				var LastPublicActivity = Grass.GetLastPublicActivity(User);
				LastActivityStampLabel.Text = "Last Updated: " + LastPublicActivity.ToString("yyyy-MM-dd HH:mm:ss");

				var LeftTime = LastPublicActivity.AddHours(24) -DateTime.Now;
				LeftTimeLabel.Text = "Left Time: " + LeftTime.ToString("hh\\:mm\\:ss");
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
				UserLabel.Text = "User: unspecified";
				UserLabel.TextColor = Color.Default;
				LastActivityStampLabel.Text = "";
				LeftTimeLabel.Text = "";
			}
		}
	}
}


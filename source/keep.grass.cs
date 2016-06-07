using System;

using Xamarin.Forms;
using Plugin.LocalNotifications;
using keep.grass.Helpers;

namespace keep.grass
{
	public class App : Application
	{
		public NavigationPage Navigation;
		public MainPage Main;

		public App()
		{
			// The root page of your application
			MainPage = Navigation = new NavigationPage
			(
				Main = new MainPage(this)
			);
			MainPage.Title = "keep.grass";
			ShowSettingsButtonOnToolbar();
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
			Main.UpdateLastPublicActivityAsync().Wait(0);
		}

		public bool IsShowSettingsButtonOnToolbar()
		{
			return 0 != Navigation.ToolbarItems.Count;
		}
		public void ShowSettingsButtonOnToolbar()
		{
			if (!IsShowSettingsButtonOnToolbar())
			{
				Navigation.ToolbarItems.Add
				(
					new ToolbarItem
					(
						"Settings",
						null,
						() =>
						{
							ShowSettingsPage();
						}
					)
				);
			}
		}

		public void HideSettingsButtonOnToolbar()
		{
			if (IsShowSettingsButtonOnToolbar())
			{
				Navigation.ToolbarItems.RemoveAt(0);
			}
		}

		public void ShowSettingsPage()
		{
			HideSettingsButtonOnToolbar();
			Navigation.PushAsync(new SettingsPage(this));
		}

		public void OnChangeSettings()
		{
			Main.UpdateInfoAsync().Wait(0);
			UpdateAlerts();
		}

		public void UpdateAlerts()
		{
			if
			(
				String.IsNullOrWhiteSpace(Settings.UserName) ||
				default(DateTime) == Main.LastPublicActivity
			)
			{
				CancelAllAlerts();
			}
			else
			{
				var Limit = Main.LastPublicActivity.AddHours(24);
				var LastPublicActivityInfo = "Last Acitivity Stamp: " +Main.LastPublicActivity.ToString("yyyy-MM-dd HH:mm:ss");
				var Now = DateTime.Now;
				int i = 0;
				foreach(var Span in Settings.AlertTimeSpanTable)
				{
					++i;
					var AlertStamp = Limit.Add(Span);
					if (Settings.GetAlert(Span) && Now < AlertStamp)
					{
						CrossLocalNotifications.Current.Show
						(
							Settings.AlertTimeSpanToDisplayName(Span),
							LastPublicActivityInfo,
							i,
							AlertStamp
						);
					}
					else
					{
						CrossLocalNotifications.Current.Cancel(i);
					}
				}
			}
		}
		public void CancelAllAlerts()
		{
			int i = 0;
			foreach(var Span in Settings.AlertTimeSpanTable)
			{
				CrossLocalNotifications.Current.Cancel(++i);
			}
		}
	}
}


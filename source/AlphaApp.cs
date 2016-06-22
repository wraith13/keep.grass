using System;

using Xamarin.Forms;
using Plugin.LocalNotifications;
using keep.grass.Helpers;

namespace keep.grass
{
	public class AlphaApp : Application
	{
		public NavigationPage Navigation;
		public AlphaMainPage Main;
		public Languages.AlphaLanguage L;

		public AlphaApp()
		{
			L = AlphaFactory.
			MainPage = Navigation = new NavigationPage
			(
				Main = AlphaFactory.makeMainPage(this)
			);
			MainPage.Title = "keep.grass";
			ShowSettingsButtonOnToolbar();
		}

		public String getLanguage()
		{
			return "ja";
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
			Navigation.PushAsync(AlphaFactory.makeSettingsPage(this));
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
				null == Main.LastPublicActivity
			)
			{
				CancelAllAlerts();
			}
			else
			{
				var Limit = Main.LastPublicActivity.Value.AddHours(24);
				var LastPublicActivityInfo = "Last Stamp: " +Main.LastPublicActivity.Value.ToString("HH:mm");
				var Now = DateTime.Now;
				int i = 0;
				foreach(var Span in Settings.AlertTimeSpanTable)
				{
					++i;
					var AlertStamp = Limit.Add(-Span);
					if (Settings.GetAlert(Span) && Now < AlertStamp)
					{
						ShowAlerts
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
		public virtual void ShowAlerts(string title, string body, int id, DateTime notifyTime)
		{
			CrossLocalNotifications.Current.Show
			(
				title,
				body,
				id,
				notifyTime
			);
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


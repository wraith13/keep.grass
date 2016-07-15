using System;

using Xamarin.Forms;
using Plugin.LocalNotifications;
using keep.grass.Helpers;
using System.Diagnostics;

namespace keep.grass
{
	public class AlphaApp : Application
	{
		public NavigationPage Navigation;
		public AlphaMainPage Main;
		public Languages.AlphaLanguage L;

		public AlphaApp()
		{
			L = AlphaFactory.MakeLanguage(this);
			MainPage = Navigation = new NavigationPage
			(
				Main = AlphaFactory.MakeMainPage(this)
			);
			MainPage.Title = "keep.grass";
		}

		public void RebuildMainPage()
		{
			Debug.WriteLine("AlphaApp::RebuildMainPage");
			Main.Rebuild();
		}

		public virtual String getLanguage()
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
			Main.StopUpdateLeftTimeTask();
		}
		protected override void OnResume()
		{
			// Handle when your app resumes
			Main.UpdateLastPublicActivityAsync().Wait(0);
		}

		public void ShowSettingsPage()
		{
			Navigation.PushAsync(AlphaFactory.MakeSettingsPage(this));
		}

		public void OnChangeSettings()
		{
			L.Update();
			Main.UpdateInfoAsync().Wait(0);
			UpdateAlerts();
		}

		public virtual void UpdateAlerts()
		{
			if
			(
				String.IsNullOrWhiteSpace(Settings.UserName) ||
				null == Main.LastPublicActivity
			)
			{
				Debug.WriteLine("AlphaApp::CancelAllAlerts");
				CancelAllAlerts();
			}
			else
			{
				Debug.WriteLine("AlphaApp::UpdateAlerts");
				var Limit = Main.LastPublicActivity.Value.AddHours(24);
				var LastPublicActivityInfo = L["Last Acitivity Stamp"] +": " +Main.LastPublicActivity.Value.ToString("yyyy-MM-dd HH:mm:ss");
				var Now = DateTime.Now;
				int i = 0;
				foreach(var Span in Settings.AlertTimeSpanTable)
				{
					++i;
					var AlertStamp = Limit.Add(-Span);
					if (Settings.GetAlert(Span) && Now < AlertStamp)
					{
						ShowAlert
						(
							Settings.AlertTimeSpanToDisplayName(L, Span),
							LastPublicActivityInfo,
							i,
							AlertStamp
						);
					}
					else
					{
                        CancelAlert(i);
					}
				}
			}
		}
		public virtual void ShowAlert(string title, string body, int id, DateTime notifyTime)
		{
			CrossLocalNotifications.Current.Show
			(
				title,
				body,
				id,
				notifyTime
			);
		}
        public virtual void CancelAlert(int id)
        {
            CrossLocalNotifications.Current.Cancel(id);
        }
        public void CancelAllAlerts()
		{
			int i = 0;
			foreach(var Span in Settings.AlertTimeSpanTable)
			{
                CancelAlert(++i);
			}
		}
	}
}


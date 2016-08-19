using System;

using Xamarin.Forms;
using keep.grass.Helpers;
using System.Diagnostics;
using System.Reflection;

namespace keep.grass
{
	public abstract class AlphaApp : Application
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
			Main.StopUpdateLeftTimeTask();
		}
		protected override void OnResume()
		{
			UpdateAlerts();
			Main.AutoUpdateLastPublicActivityAsync().Wait(0);
		}

		public void ShowMainPage()
		{
			Navigation.PopToRootAsync();
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
			CancelAllAlerts();
			if
			(
				String.IsNullOrWhiteSpace(Settings.UserName) ||
				null == Main.LastPublicActivity
			)
			{
				Debug.WriteLine("AlphaApp::CancelAllAlerts");
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
		public abstract void ShowAlert(string title, string body, int id, DateTime notifyTime);
		public abstract void CancelAlert(int id);

		public virtual void CancelAllAlerts()
		{
			int i = 0;
			foreach(var Span in Settings.AlertTimeSpanTable)
			{
                CancelAlert(++i);
			}
		}

		public virtual ImageSource GetImageSource(string image)
		{
            return ImageSource.FromResource
            (
                "keep.grass.Images." +image,
                typeof(AlphaApp).GetTypeInfo().Assembly
            );
		}
		public virtual ImageSource GetApplicationImageSource()
		{
			return GetImageSource("keep.grass.120.png");
		}
		public virtual ImageSource GetWraithImageSource()
		{
			return GetImageSource("wraith13.120.png");
		}
		public virtual ImageSource GetGitHubImageSource()
		{
			return GetImageSource("GitHub-Mark.120.png");
		}

		public virtual Uri GetApplicationStoreUri()
		{
			return null;
		}
	}
}


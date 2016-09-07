using System;

using Xamarin.Forms;
using keep.grass.Helpers;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace keep.grass
{
	public abstract class AlphaApp : Application
	{
		public NavigationPage Navigation;
		public AlphaMainPage Main;
		AlphaDomain Domain = AlphaFactory.MakeSureDomain();

		public AlphaApp()
		{
			MainPage = Navigation = new NavigationPage
			(
				Main = AlphaFactory.MakeMainPage()
			);
			MainPage.Title = "keep.grass";
		}

		public void RebuildMainPage()
		{
			Debug.WriteLine("AlphaApp::RebuildMainPage");
			Main.Rebuild();
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
			Domain.UpdateAlerts();
			Task
				.Delay(TimeSpan.FromMilliseconds(3000))
				.ContinueWith
				(
					async (t) => await Domain.AutoUpdateLastPublicActivityAsync(),
					TaskScheduler.FromCurrentSynchronizationContext()
				);
		}

		public void ShowMainPage()
		{
			Navigation.PopToRootAsync();
		}

		public void ShowSettingsPage()
		{
			Navigation.PushAsync(AlphaFactory.MakeSettingsPage());
		}

		public void OnChangeSettings()
		{
			AlphaFactory.MakeSureLanguage().Update();
			Main.UpdateInfoAsync().Wait(0);
			Domain.UpdateAlerts();
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
	}
}


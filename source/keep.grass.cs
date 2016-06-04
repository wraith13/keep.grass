using System;

using Xamarin.Forms;

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

		public void OnChangeSettings()
		{
			Main.UpdateInfoAsync().Wait(0);
		}
	}
}


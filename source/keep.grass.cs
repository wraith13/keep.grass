using System;

using Xamarin.Forms;

namespace keep.grass
{
	public class App : Application
	{
		public NavigationPage navigation;

		public App()
		{
			// The root page of your application
			MainPage = navigation = new NavigationPage
			(
				new MainPage(this)
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
		}

	}
}


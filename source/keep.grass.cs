using System;

using Xamarin.Forms;

namespace keep.grass
{
	public class App : Application
	{
		NavigationPage navigation;
		public App()
		{
			// The root page of your application
			MainPage = navigation = new NavigationPage
			(
				new ContentPage {
					Content = new StackLayout {
						VerticalOptions = LayoutOptions.Center,
						Children = {
							new Label {
								XAlign = TextAlignment.Center,
								Text = "Welcome to Xamarin Forms!",
							},
							new Button {
								Text = "Settings",
								Command = new Command(o => navigation.PushAsync(new SettingsPage())),
							},
						}
					}
				}
			);
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


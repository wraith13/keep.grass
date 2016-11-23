using System;

using Xamarin.Forms;
using keep.grass.Helpers;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;

namespace keep.grass
{
	public abstract class AlphaApp : Application
	{
		public NavigationPage Navigation;
		public AlphaMainPage Main;
		AlphaDomain Domain = AlphaFactory.MakeSureDomain();

		public AlphaApp()
		{
			AlphaFactory.SetApp(this);

			MainPage = Navigation = new NavigationPage
			(
				Main = AlphaFactory.MakeMainPage()
			);
			MainPage.Title = "keep.grass";
		}

		public void RebuildMainPage()
		{
			Debug.WriteLine("AlphaApp::RebuildMainPage");
			Main.Build();
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
		}

		public void ShowMainPage()
		{
			Navigation.PopToRootAsync();
		}

		public void ShowDetailPage(string User)
		{
			Navigation.PushAsync(new AlphaDetailPage(User));
		}

		public void ShowSettingsPage()
		{
			Navigation.PushAsync(AlphaFactory.MakeSettingsPage());
		}

		public void OnChangeSettings()
		{
			AlphaFactory.MakeSureLanguage().Update();
			Main.UpdateInfoAsync();
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
		public virtual ImageSource GetRightImageSource()
		{
			return GetImageSource("right.120.png");
		}
		public virtual ImageSource GetRefreshImageSource()
		{
			return GetImageSource("refresh.120.png");
		}
		public virtual ImageSource GetExportImageSource()
		{
			return GetImageSource("export.120.png");
		}
		public virtual Stream GetFontStream()
		{
			return typeof(AlphaApp).GetTypeInfo().Assembly.GetManifestResourceStream("keep.grass.Fonts.NotoSansCJKjp-Regular.otf");
		}
	}
}


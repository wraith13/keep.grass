using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using ImageCircle.Forms.Plugin.Droid;

namespace keep.grass.Droid
{
	[
		Activity
		(
			Label = "keep.grass",
			Icon = "@drawable/icon",
			MainLauncher = true,
			ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
			Theme = "@android:style/Theme.Holo.Light"
		)
	]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		static AlphaApp App;
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);
			LoadApplication(MakeSureApp());
		}

		protected override void OnResume()
		{
			base.OnResume();

			if ("alarm" == (Intent.GetStringExtra("type") ?? ""))
			{
				Intent.RemoveExtra("type");
				App.ShowMainPage();
			}
			App?.Main?.StartUpdateLeftTimeTask();
		}

		public static AlphaApp MakeSureApp()
		{
			if (null == App)
			{
				//global::Xamarin.Forms.Forms.Init(this, bundle);
				ImageCircleRenderer.Init();
				OmegaFactory.MakeSureInit();
				App = AlphaFactory.MakeSureApp();
			}
			return App;
		}
	}
}


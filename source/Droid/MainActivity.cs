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
		AlphaApp App;
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);
			ImageCircleRenderer.Init();

			OmegaFactory.Init();

			LoadApplication(App = AlphaFactory.MakeApp());
		}

		protected override void OnResume()
		{
			base.OnResume();

			if (0 < this.Intent.GetIntExtra("id", -1))
			{
				//App.ShowMainPage();
			}
		}
	}
}


using System.Diagnostics;
using Android.App;
using Android.Content;
using Android.Graphics;
using Xamarin.Forms;

namespace keep.grass.Droid
{
	[Service]
	public class UpdateIntentService :IntentService
	{
		public override void OnCreate()
		{
			base.OnCreate();
			global::Xamarin.Forms.Forms.Init(this, null);
		}

		protected override void OnHandleIntent(Intent intent)
		{
			Debug.WriteLine("UpdateIntentService::OnHandleIntent()");
			try
			{
				UpdateLastPublicActivity(intent);
			}
			finally
			{
				Android.Support.V4.Content.WakefulBroadcastReceiver.CompleteWakefulIntent(intent);
			}
		}

		void UpdateLastPublicActivity(Intent intent)
		{
			OmegaFactory.MakeSureInit();
			var Domain = AlphaFactory.MakeSureDomain() as OmegaDomain;
			try
			{
				Domain.ThreadContext.Value = this;
				Domain.AutoUpdateLastPublicActivityAsync().Wait();
			}
			finally
			{
				Domain.ThreadContext.Value = null;
			}
		}
	}
}


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
		protected override void OnHandleIntent(Intent intent)
		{
			Debug.WriteLine("UpdateIntentService::OnHandleIntent()");
			try
			{
				UpdateLastPublicActivity(ApplicationContext, intent);
			}
			finally
			{
				Android.Support.V4.Content.WakefulBroadcastReceiver.CompleteWakefulIntent(intent);
			}
		}
		public void UpdateLastPublicActivity(Context context, Intent intent)
		{
		}
	}
}


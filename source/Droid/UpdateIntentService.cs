using System.Diagnostics;
using Android.App;
using Android.Content;
using Android.Graphics;
using Xamarin.Forms;

namespace keep.grass.Droid
{
	[Service]
	public class AlarmIntentService :IntentService
	{
		protected override void OnHandleIntent(Intent intent)
		{
			Debug.WriteLine("AlarmIntentService::OnHandleIntent()");
			try
			{
				ShowNotification(ApplicationContext, intent);
			}
			finally
			{
				Android.Support.V4.Content.WakefulBroadcastReceiver.CompleteWakefulIntent(intent);
			}
		}
		public void ShowNotification(Context context, Intent intent)
		{
			var id = intent.GetIntExtra("id", 0);
			var title = intent.GetStringExtra("title");
			var body = intent.GetStringExtra("message");

			var main_intent = new Intent(context, typeof(MainActivity));
			main_intent.PutExtra("type", "alarm");
			main_intent.PutExtra("id", id);

			var builder = new Notification.Builder(Forms.Context);
			//builder.SetGroup("keep.grass");
			builder.SetTicker(title);
			builder.SetContentTitle(title);
			builder.SetContentText(body);
			builder.SetContentIntent
			(
				PendingIntent.GetActivity
				(
					context,
					id,
					main_intent,
					PendingIntentFlags.UpdateCurrent
				)
			);
			builder.SetSmallIcon(Resource.Drawable.icon);
			builder.SetLargeIcon(BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.icon));
			builder.SetDefaults(NotificationDefaults.All);
			builder.SetAutoCancel(true);

			((NotificationManager)context.GetSystemService(NotificationService))
				.Notify(id, builder.Build());
		}
	}
}


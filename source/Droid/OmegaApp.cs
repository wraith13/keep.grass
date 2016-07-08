using System;
using Java.Util;
using Android.App;
using Android.Content;

namespace keep.grass.Droid
{
	public class OmegaApp : AlphaApp
	{
		public OmegaApp()
		{
		}

		public override string getLanguage()
		{
			return Locale.Default.ToString().Split('_')[0];
		}

		public override void ShowAlert(string title, string body, int id, DateTime notifyTime)
		{
			var builder = new Notification.Builder(Xamarin.Forms.Forms.Context);
			builder.SetContentTitle(title);
			builder.SetContentText(body);
			builder.SetGroup("keep.grass.alert");
			//builder.SetLargeIcon();

			var notification = builder.Build();
			var notificationIntent = new Intent(Xamarin.Forms.Forms.Context, typeof(MainActivity));
			notificationIntent.PutExtra(NotificationPublisher.NOTIFICATION_ID, 1);
		    notificationIntent.PutExtra(NotificationPublisher.NOTIFICATION, notification);
		    PendingIntent pendingIntent = PendingIntent.GetBroadcast(this, 0, notificationIntent, PendingIntent.FLAG_UPDATE_CURRENT);

			long futureInMillis = SystemClock.elapsedRealtime() + delay;
			AlarmManager alarmManager = (AlarmManager)getSystemService(Context.ALARM_SERVICE);
			alarmManager.set(AlarmManager.ELAPSED_REALTIME_WAKEUP, futureInMillis, pendingIntent);
		
		}
		public override void CancelAlert(int id)
		{
		}
	}
}


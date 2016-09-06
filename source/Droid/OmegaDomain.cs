using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Xamarin.Forms;

namespace keep.grass.Droid
{
	public class OmegaDomain : AlphaDomain
	{
		public ThreadLocal<Context> ThreadContext = new ThreadLocal<Context>(() => null);
		public Context Context
		{
			get
			{
				return ThreadContext.Value ?? Forms.Context;
			}
		}

		public AlarmManager GetAlarmManager()
		{
			return ((AlarmManager)Context.GetSystemService(Context.AlarmService));
		}
		public PendingIntent MakeAlarmIntent(int id, string title = null, string body = null)
		{
			var alarmIntent = new Intent(Context, typeof(AlarmWakefulReceiver));
			alarmIntent.PutExtra("id", id);
			if (null != title)
			{
				alarmIntent.PutExtra("title", title);
			}
			if (null != body)
			{
				alarmIntent.PutExtra("message", body);
			}
			var result = PendingIntent.GetBroadcast
			  (
				Forms.Context,
				id,
				alarmIntent,
				PendingIntentFlags.UpdateCurrent
			);
			return result;
		}
		public override void ShowAlert(string title, string body, int id, DateTime notifyTime)
		{
			GetAlarmManager().Set
			(
				AlarmType.ElapsedRealtime,
				SystemClock.ElapsedRealtime() + (long)((notifyTime - DateTime.Now).TotalMilliseconds),
				MakeAlarmIntent(id, title, body)
		   );
		}
		public override void CancelAlert(int id)
		{
			GetAlarmManager().Cancel
			(
				MakeAlarmIntent(id)
			);
		}
		public override void CancelAllAlerts()
		{
			((NotificationManager)Forms.Context.GetSystemService(Context.NotificationService)).CancelAll();
		}

		public override Uri GetApplicationStoreUri()
		{
			//	暫定実装。これは実際には twitter 公式アプリの URL
			return new Uri("https://play.google.com/store/apps/details?id=com.twitter.android&hl=ja");
		}
	}
}


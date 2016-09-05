using System;
using Android.App;
using Android.Content;
using Android.OS;
using Xamarin.Forms;

namespace keep.grass.Droid
{
	public class OmegaDomain : AlphaDomain
	{
		//	このクラス内の Forms.Context の代わりに外部から Context を指定する形にすること！

		public AlarmManager GetAlarmManager()
		{
			return ((AlarmManager)Forms.Context.GetSystemService(Context.AlarmService));
		}
		public PendingIntent MakeAlarmIntent(int id, string title = null, string body = null)
		{
			var alarmIntent = new Intent(Forms.Context, typeof(AlarmWakefulReceiver));
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
	}
}


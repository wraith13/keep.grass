using System;
using System.Collections.Generic;
using Java.Util;
using Android.App;
using Android.Content;
using Android.OS;
using Xamarin.Forms;

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

		public AlarmManager GetAlarmManager()
		{
			return ((AlarmManager)Forms.Context.GetSystemService(Context.AlarmService));
		}
		public PendingIntent MakeAlarmIntent(string title, string body, int id)
		{
			var alarmIntent = new Intent(Forms.Context, typeof(AlarmReceiver));
			if (null != body)
			{
				alarmIntent.PutExtra("message", body);
			}
			if (null != title)
			{
				alarmIntent.PutExtra("title", title);
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
	            SystemClock.ElapsedRealtime() +(long)((notifyTime -DateTime.Now).TotalMilliseconds),
				MakeAlarmIntent(title, body, id)
           );
		}
		public override void CancelAlert(int id)
		{
			GetAlarmManager().Cancel
            (
				MakeAlarmIntent(null, null, id)
			);
		}
	}
}


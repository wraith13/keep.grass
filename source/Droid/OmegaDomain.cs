using System;
using System.Linq;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using keep.grass.Helpers;
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

		const int BaseUpdateId = 1000;

		public static long ToAndroidTicks(DateTime at)
		{
			return SystemClock.ElapsedRealtime() +
				((long)(at -DateTime.Now).TotalMilliseconds);
		}

		public override void UpdateAlerts(DateTime LastPublicActivity)
		{
			base.UpdateAlerts(LastPublicActivity);
			if
			(
				default(DateTime) == LastPublicActivity
			)
			{
				System.Diagnostics.Debug.WriteLine("OmegaDomain::CancelAllAlerts");
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("OmegaDomain::UpdateAlerts");
				var Limit = LastPublicActivity.AddHours(24);
				var Now = DateTime.Now;
				int id = BaseUpdateId;
				foreach
				(
					var Span in
						Settings.AlertTimeSpanTable
							.Union(Enumerable.Range(0, 24).Select(i => TimeSpan.FromHours(i)))
							.Where(i => Now < Limit.Add(-i).AddSeconds(-300)) // 基準時刻が過去あるいは5分以内場合は実行しない
				)
				{
					//	基準時刻の90秒前に
					var updateAt = Limit -(Span + TimeSpan.FromSeconds(90));

					GetAlarmManager().Set
					(
						AlarmType.ElapsedRealtime,
						ToAndroidTicks(updateAt),
						PendingIntent.GetBroadcast
						(
							Forms.Context,
							id++,
							new Intent(Context, typeof(UpdateWakefulReceiver)),
							PendingIntentFlags.UpdateCurrent
						)
				   );
				}
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
				ToAndroidTicks(notifyTime),
				MakeAlarmIntent(id, title, body)
		   	);
		}
		public override void CancelAlert(int id)
		{
			GetAlarmManager().Cancel
			(
				MakeAlarmIntent(id)
			);
			GetAlarmManager().Cancel
			(
				MakeAlarmIntent(id +BaseUpdateId)
			);
		}
		public override void CancelAllAlerts()
		{
			((NotificationManager)Forms.Context.GetSystemService(Context.NotificationService)).CancelAll();
		}

		public override Uri GetApplicationStoreUri()
		{
			return new Uri("https://play.google.com/store/apps/details?id=net.trickpalace.keep_grass&hl=ja");
		}
	}
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Xamarin.Forms;

namespace keep.grass.Droid
{
	[BroadcastReceiver]
	public class AlarmReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			var id = intent.GetIntExtra("id", 0);
			var title = intent.GetStringExtra("title");
			var body = intent.GetStringExtra("message");

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
					new Intent(context, typeof(MainActivity)),
					PendingIntentFlags.UpdateCurrent
				)
			);
			builder.SetSmallIcon(Resource.Drawable.icon);
			builder.SetLargeIcon(BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.icon));
			builder.SetDefaults(NotificationDefaults.All);
			builder.SetAutoCancel(true);

			((NotificationManager)context.GetSystemService(Service.NotificationService))
				.Notify(id, builder.Build());
		}
	}
}


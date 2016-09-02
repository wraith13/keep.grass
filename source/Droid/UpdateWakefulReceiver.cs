using System.Diagnostics;
using Android.Content;
using Android.Support.V4.Content;

namespace keep.grass.Droid
{
	[BroadcastReceiver]
	public class UpdateWakefulReceiver : WakefulBroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			Debug.WriteLine("UpdateWakefulReceiver::OnReceive()");
			StartWakefulService(context, new Intent(context, typeof(UpdateIntentService)));
		}
	}
}


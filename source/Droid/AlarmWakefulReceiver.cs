using System.Diagnostics;
using Android.Content;
using Android.Support.V4.Content;

namespace keep.grass.Droid
{
    [BroadcastReceiver]
    public class AlarmWakefulReceiver : WakefulBroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Debug.WriteLine("AlarmWakefulReceiver::OnReceive()");
            var intent2 = new Intent(context, typeof(AlarmIntentService));
            intent2.PutExtra("id", intent.GetIntExtra("id", 0));
            intent2.PutExtra("title", intent.GetStringExtra("title"));
            intent2.PutExtra("message", intent.GetStringExtra("message"));
            StartWakefulService(context, intent2);
        }
    }
}


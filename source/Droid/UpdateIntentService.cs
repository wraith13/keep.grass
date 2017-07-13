using System.Diagnostics;
using Android.App;
using Android.Content;
using Android.Graphics;
using Xamarin.Forms;
using keep.grass.Domain;

namespace keep.grass.Droid
{
    [Service]
    public class UpdateIntentService : IntentService
    {
        public override void OnCreate()
        {
            base.OnCreate();
            global::Xamarin.Forms.Forms.Init(this, null);
        }

        protected override void OnHandleIntent(Intent intent)
        {
            Debug.WriteLine("UpdateIntentService::OnHandleIntent()");
            try
            {
                UpdateLastPublicActivity(intent);
            }
            finally
            {
                Android.Support.V4.Content.WakefulBroadcastReceiver.CompleteWakefulIntent(intent);
            }
        }

        void UpdateLastPublicActivity(Intent intent)
        {
            OmegaDomainFactory.MakeSureInit();
            OmegaAppFactory.MakeSureInit();
            var Domain = AlphaDomainFactory.MakeSureDomain() as OmegaDomain;
            try
            {
                Domain.ThreadContext.Value = this;
                Domain.BackgroundUpdateLastPublicActivityAsync().Wait();
            }
            finally
            {
                Domain.ThreadContext.Value = null;
            }
        }
    }
}


using AppKit;
using Foundation;
using Xamarin.Forms;
using keep.grass;

namespace keep.grass.Mac
{
    [Register("AppDelegate")]
    public class AppDelegate : NSApplicationDelegate
    {
        AlphaApp App;

        public AppDelegate()
        {
        }

        public AlphaApp MakeSureApp()
        {
            if (null == App)
            {
                global::Xamarin.Forms.Forms.Init();
                ImageCircleRenderer.Init();
                OmegaFactory.MakeSureInit();
                App = AlphaFactory.MakeSureApp();
            }
            return App;
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            Forms.Init();
            LoadApplication(MakeSureApp());
            base.DidFinishLaunching(notification);
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }
    }
}

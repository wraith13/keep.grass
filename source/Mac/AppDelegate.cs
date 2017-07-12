using AppKit;
using Foundation;
using Xamarin.Forms;
using keep.grass;
using Xamarin.Forms.Platform.MacOS;
using keep.grass.App;

namespace keep.grass.Mac
{
    [Register("AppDelegate")]
    //public class AppDelegate : NSApplicationDelegate
    public class AppDelegate : FormsApplicationDelegate
    {
        NSWindow _window;
        public AppDelegate()
        {
            var style = NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled;

            var rect = new CoreGraphics.CGRect(200, 1000, 1024, 768);
            _window = new NSWindow(rect, style, NSBackingStore.Buffered, false);
            _window.Title = "Xamarin.Forms Mac";
            _window.TitleVisibility = NSWindowTitleVisibility.Hidden;
        }

        public override NSWindow MainWindow
        {
            get { return _window; }
        }

        AlphaApp App;
        public AlphaApp MakeSureApp()
        {
            if (null == App)
            {
                global::Xamarin.Forms.Forms.Init();
                //ImageCircleRenderer.Init();
                OmegaDomainFactory.MakeSureInit();
                OmegaAppFactory.MakeSureInit();
                App = AlphaAppFactory.MakeSureApp();
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

using System;
using WatchKit;
using WatchConnectivity;
using Foundation;
using SkiaSharp;
using keep.grass.Domain;
using RuyiJinguBang;

namespace keep.grass.iOS.keep.grassExtension
{
    public class SessionReceiver: WCSessionDelegate
    {
        public override void DidReceiveApplicationContext(WCSession session, NSDictionary<NSString, NSObject> applicationContext)
        {
            Console.WriteLine($"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@DidReceiveApplicationContext");
            var User = (applicationContext.ValueForKey(new NSString("User")) as NSString)?.ToString();
            Console.WriteLine($"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@User: {User}");
            var LastPublicActivity = (applicationContext.ValueForKey(new NSString("LastPublicActivity")) as NSDate)?.ToDateTime() ?? default(DateTime);
            Console.WriteLine($"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@LastPublicActivity: {LastPublicActivity.ToString()}");
        }

        static SessionReceiver Instance = null;
        public static SessionReceiver MakeSure()
        {
            return Instance ?? (Instance = new SessionReceiver());
        }
    }
    public partial class InterfaceController : WKInterfaceController
    {
        protected InterfaceController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void Awake(NSObject context)
        {
            base.Awake(context);

            // Configure interface objects here.
            Console.WriteLine("{0} awake with context", this);

            //FontSource = AlphaFactory.GetApp().GetFontStream();
            //FontStream = new SKManagedStream(FontSource);
            //Font = SKTypeface.FromStream(FontStream);
            //Drawer.Font = Font;
            Drawer.OnUpdate = d => Update();

            if (WCSession.IsSupported)
            {
                var DefaultSession = WCSession.DefaultSession;
                DefaultSession.Delegate = SessionReceiver.MakeSure();
                DefaultSession.ActivateSession();
            }
        }

        public override void WillActivate()
        {
            // This method is called when the watch view controller is about to be visible to the user.
            Console.WriteLine("{0} will activate", this);
        }

        public override void DidAppear()
        {
            base.DidAppear();
            if (WCSession.IsSupported)
            {
                //WatchCanvas.SetImage();
            }
            else
            {
                PresentAlertController
                (
                    "iPhone と通信できません",
                    "iPhone との接続状態を確認してください。",
                    WKAlertControllerStyle.Alert,
                    new[] { WKAlertAction.Create("OK", WKAlertActionStyle.Default, () => this.DismissController()), }
                );
            }
        }

        public override void DidDeactivate()
        {
            // This method is called when the watch view controller is no longer visible to the user.
            Console.WriteLine("{0} did deactivate", this);
        }

        public AlphaDrawer Drawer = new AlphaDrawer();
        System.IO.Stream FontSource;
        SKManagedStream FontStream;
        protected SKTypeface Font;

        /*
        public void Dispose()
        {
            Drawer.Font = null;
            Font?.Dispose();
            Font = null;
            FontStream?.Dispose();
            FontStream = null;
            FontSource?.Dispose();
            FontSource = null;
        }
        */

        public virtual float GetPhysicalPixelRate()
        {
            return 4.0f; // この数字は適当。本来はちゃんとデバイスごとの物理解像度/論理解像度を取得するべき
        }
        public virtual SKColorType GetDeviceColorType()
        {
            return SKColorType.Rgba8888;
        }
        double Width => WatchCanvas.AccessibilityFrame.Width;
        double Height => WatchCanvas.AccessibilityFrame.Height;
        public void Update()
        {
            //var Radius = (DrawGraphSize / 2.0f) - (Margin * PhysicalPixelRate);
            //var Center = new SKPoint(DrawGraphSize / 2.0f, DrawGraphSize / 2.0f);
            if (0.0 < Width && 0.0 < Height)
            {
                using (var Surface = SKSurface.Create((int)(Width * GetPhysicalPixelRate()), (int)(Height * GetPhysicalPixelRate()), GetDeviceColorType(), SKAlphaType.Premul))
                {
                    Draw(Surface.Canvas);
                    WatchCanvas.SetImage(NSData.FromArray(Surface.Snapshot().Encode().ToArray()));
                    /*
                    var CanvasImageData = Surface.Snapshot().Encode();
                    Device.BeginInvokeOnMainThread
                    (
                        () =>
                        {
                            Source = ImageSource.FromStream(() => CanvasImageData.AsStream());
                        }
                    );
                    */
                }
            }
        }
        public virtual void Draw(SKCanvas Canvas)
        {
            Drawer.Width = Width;
            Drawer.Height = Height;
            Drawer.Draw(Canvas);
        }

        public void AppliedTheme(AlphaTheme Theme)
        {
            Drawer.BackgroundColor = Theme.BackgroundColor;
            Drawer.IsInvalidCanvas = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Foundation;
using UIKit;
using WatchConnectivity;
using keep.grass.Domain;
using keep.grass.App;

namespace keep.grass.iOS
{
    public class SessionReceiver : WCSessionDelegate
    {
        static SessionReceiver Instance = null;
        public static SessionReceiver MakeSure()
        {
            return Instance ?? (Instance = new SessionReceiver());
        }
    }
    public class OmegaApp : AlphaApp
    {
        public OmegaApp()
        {
        }

        public override void OnChangeSettings()
        {
            base.OnChangeSettings();

            if (WCSession.IsSupported)
            {
                var DefaultSession = WCSession.DefaultSession;
                DefaultSession.Delegate = SessionReceiver.MakeSure();
                DefaultSession.ActivateSession();
                var Dic = new NSDictionary<NSString, NSObject>(new NSString("User"), new NSString(Settings.UserName ?? ""));
                var Error = default(NSError);
                WCSession.DefaultSession.UpdateApplicationContext(Dic, out Error);
                if (null != Error)
                {
                    Debug.WriteLine(Error.ToString());
                }
            }
        }
    }
}


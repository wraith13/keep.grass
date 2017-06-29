using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using WatchConnectivity;
using keep.grass.Domain;
using keep.grass.App;

namespace keep.grass.iOS
{
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
                var Dic = new NSDictionary<NSString, NSObject>(new NSString("User"), new NSString(Settings.UserName ?? ""));
                var Error = new NSError();
                WCSession.DefaultSession.UpdateApplicationContext(Dic, out Error);
            }
        }
    }
}


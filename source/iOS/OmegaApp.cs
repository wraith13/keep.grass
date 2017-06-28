using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using keep.grass.App;
using WatchConnectivity;

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
                
            }
        }
    }
}


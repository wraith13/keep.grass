﻿using System;

using WatchKit;
using WatchConnectivity;
using Foundation;
using keep.grass.Domain;

namespace keep.grass.iOS.keep.grassExtension
{
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

            Console.WriteLine($"@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            if (WCSession.IsSupported)
            {
                //WatchCanvas.SetImage();
                //Console.WriteLine($"User: {Settings.UserName ?? "Undefined"}");
                PresentAlertController
                (
                    "Title",
                    "IsSuppoeted!!!",
                    WKAlertControllerStyle.Alert,
                    new[] { WKAlertAction.Create("OK", WKAlertActionStyle.Default, () => this.PopController()), }
                );
            }
            else
            {
                PresentAlertController
                (
                    "iPhone と通信できません",
                    "iPhone との接続状態を確認してください。",
                    WKAlertControllerStyle.Alert,
                    new[] { WKAlertAction.Create("OK", WKAlertActionStyle.Default, () => this.PopController()), }
                );
            }
        }

        public override void WillActivate()
        {
            // This method is called when the watch view controller is about to be visible to the user.
            Console.WriteLine("{0} will activate", this);
        }

        public override void DidDeactivate()
        {
            // This method is called when the watch view controller is no longer visible to the user.
            Console.WriteLine("{0} did deactivate", this);
        }
    }
}

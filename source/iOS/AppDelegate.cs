using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using UIKit;

namespace keep.grass.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		AlphaApp App;
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();
			ImageCircleRenderer.Init();

			OmegaFactory.Init();

			LoadApplication(App = AlphaFactory.MakeApp());

			// copy from http://www.knowing.net/index.php/2014/07/03/local-notifications-in-ios-8-with-xamarin/
			var settings = UIUserNotificationSettings.GetSettingsForTypes
				(
					UIUserNotificationType.Alert
					| UIUserNotificationType.Badge
					| UIUserNotificationType.Sound,
					new NSSet()
				);
			UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);

			return base.FinishedLaunching(app, options);
		}

		public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
		{
			App.ShowMainPage();
		}
	}
}


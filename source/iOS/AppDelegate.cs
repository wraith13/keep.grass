using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);

			return base.FinishedLaunching(app, options);
		}

		public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
		{
			//	暫定実装。PCL側のコードの構造を大幅に修正して UI 要素に引き摺られない形にすること。
			var LastActivityStampLabel = App?.Main?.LastActivityStampLabel.Text ?? "null";
			App?.Main?.AutoUpdateLastPublicActivityAsync().Wait();
			var NewLastActivityStampLabel = App?.Main?.LastActivityStampLabel.Text ?? "null";
			completionHandler
			(
				App.L["Error"] == NewLastActivityStampLabel ?
					UIBackgroundFetchResult.Failed :
					(
						LastActivityStampLabel == NewLastActivityStampLabel ?
							UIBackgroundFetchResult.NoData :
							UIBackgroundFetchResult.NewData
					)
			);
		}

		public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
		{
			App.ShowMainPage();
		}
	}
}


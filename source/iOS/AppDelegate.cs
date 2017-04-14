using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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
			LoadApplication(MakeSureApp());

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

		public override void WillEnterForeground(UIApplication uiApplication)
		{
			base.WillEnterForeground(uiApplication);
			App?.Main?.StartUpdateLeftTimeTask();
		}

		public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
		{
			OmegaFactory.MakeSureInit();
			AlphaFactory.MakeSureDomain().BackgroundUpdateLastPublicActivityAsync().ContinueWith
	        (
	            t =>
					completionHandler(UIBackgroundFetchResult.NoData)
					//	UIBackgroundFetchResult.NewData を返すと呼ばれなくなるフシがあるので必ず UIBackgroundFetchResult.NoData
					//	を返しておくようにしてみる。
           	);
		}

		public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
		{
			MakeSureApp().ShowMainPage();
		}
	}
}


using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace keep.grass.iOS
{
	public class OmegaApp : AlphaApp
	{
		public OmegaApp()
		{
		}

		public override void ShowAlert(string title, string body, int id, DateTime notifyTime)
		{
			CancelAlert(id);
			UIApplication.SharedApplication.ScheduleLocalNotification
			(
				new UILocalNotification
				{
					//AlertTitle = title, // iOS 9 からは通知センターでしかこの AlertTitle は有効にならず、それ以外の箇所ではアプリ名が代わりに表示され使い物にならない。
					AlertBody = title +"\r\n" +body,
					FireDate = NSDate.Now.AddSeconds((notifyTime - DateTime.Now).TotalSeconds),
					TimeZone = NSTimeZone.DefaultTimeZone,
					UserInfo = new NSDictionary("id", id.ToString()),
				}
			);
		}
		public override void CancelAlert(int id)
		{
			foreach
			(
				var notification
				in UIApplication.SharedApplication.ScheduledLocalNotifications
					.Where
					(
						n =>
							n.UserInfo.ContainsKey(new NSString("id")) &&
							n.UserInfo.ValueForKey(new NSString("id")).ToString() == id.ToString()
					)
			)
			{
				UIApplication.SharedApplication.CancelLocalNotification(notification);
			}
		}
		public override void CancelAllAlerts()
		{
			UIApplication.SharedApplication.CancelAllLocalNotifications();
		}
		public override Uri GetApplicationStoreUri()
		{
			//	暫定実装。これは実際には twitter 公式アプリの URL
			return new Uri("https://itunes.apple.com/jp/app/twitter/id333903271?mt=8");
		}
	}
}


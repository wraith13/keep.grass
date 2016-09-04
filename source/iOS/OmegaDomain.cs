using System;
using System.Linq;
using Foundation;
using UIKit;

namespace keep.grass.iOS
{
	public class OmegaDomain : AlphaDomain
	{
		public override void ShowAlert(string title, string body, int id, DateTime notifyTime)
		{
			CancelAlert(id);
			UIApplication.SharedApplication.ScheduleLocalNotification
			(
				new UILocalNotification
				{
					//AlertTitle = title, // iOS 9 からは通知センターでしかこの AlertTitle は有効にならず、それ以外の箇所ではアプリ名が代わりに表示され使い物にならない。
					AlertBody = title + "\r\n" + body,
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
	}
}


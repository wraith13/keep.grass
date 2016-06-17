using System;
namespace keep.grass.iOS
{
	public class CustomApp : AlphaApp
	{
		public CustomApp()
		{
		}

		public override void ShowAlerts(string title, string body, int id, DateTime notifyTime)
		{
			base.ShowAlerts
		    (
				"keep.grass",
				title + ",\r\n" + body,
				id,
				notifyTime
		   );
		}
	}
}


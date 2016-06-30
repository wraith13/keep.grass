using System;
using Foundation;

namespace keep.grass.iOS
{
	public class OmegaApp : AlphaApp
	{
		public OmegaApp()
		{
		}

		public override string getLanguage()
		{
			return NSLocale.PreferredLanguages[0].Split('-')[0];
		}

		public override void ShowAlert(string title, string body, int id, DateTime notifyTime)
		{
			base.ShowAlert
		    (
				"keep.grass",
				title + ",\r\n" + body,
				id,
				notifyTime
		   );
		}
	}
}


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

		public override Uri GetApplicationStoreUri()
		{
			//	暫定実装。これは実際には twitter 公式アプリの URL
			return new Uri("https://itunes.apple.com/jp/app/twitter/id333903271?mt=8");
		}
	}
}


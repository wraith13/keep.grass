using System;
using System.Collections.Generic;
using Java.Util;
using Android.App;
using Android.Content;
using Android.OS;
using Xamarin.Forms;

namespace keep.grass.Droid
{
	public class OmegaApp : AlphaApp
	{
		public OmegaApp()
		{
		}

		public override Uri GetApplicationStoreUri()
		{
			//	暫定実装。これは実際には twitter 公式アプリの URL
			return new Uri("https://play.google.com/store/apps/details?id=com.twitter.android&hl=ja");
		}
	}
}


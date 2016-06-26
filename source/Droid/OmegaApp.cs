using System;
using Java.Util;

namespace keep.grass.Droid
{
	public class OmegaApp : AlphaApp
	{
		public OmegaApp()
		{
		}

		public override string getLanguage()
		{
			return Locale.Default.ToString().Split('_')[0];
		}
	}
}


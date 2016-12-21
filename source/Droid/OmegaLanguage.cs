using System;
using Java.Util;

namespace keep.grass.Droid
{
	public class OmegaLanguage : keep.grass.Languages.AlphaLanguage
	{
		public override string getLanguage()
		{
			return Locale.Default.Language.Split('-')[0];
		}
	}
}


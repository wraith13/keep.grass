using System;
using Foundation;

namespace keep.grass.iOS
{
    public class OmegaLanguage : keep.grass.Languages.AlphaLanguage
    {
        public override string getLanguage()
        {
            return NSLocale.PreferredLanguages[0].Split('-')[0];
        }
    }
}


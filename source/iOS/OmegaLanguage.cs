using System;
using Foundation;
using keep.grass.Domain;

namespace keep.grass.iOS
{
    public class OmegaLanguage : AlphaLanguage
    {
        public override string getLanguage()
        {
            return NSLocale.PreferredLanguages[0].Split('-')[0];
        }
    }
}


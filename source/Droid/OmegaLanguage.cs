using System;
using Java.Util;
using keep.grass.Domain;

namespace keep.grass.Droid
{
    public class OmegaLanguage : AlphaLanguage
    {
        public override string getLanguage()
        {
            return Locale.Default.Language.Split('-')[0];
        }
    }
}


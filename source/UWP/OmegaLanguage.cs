using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using keep.grass.Domain;

namespace keep.grass.UWP
{
    class OmegaLanguage : AlphaLanguage
    {
        public override string getLanguage()
        {
            return Windows.System.UserProfile.GlobalizationPreferences.Languages[0].Split('-')[0];
        }
    }
}

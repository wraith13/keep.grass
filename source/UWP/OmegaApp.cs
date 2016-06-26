using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace keep.grass.UWP
{
    public class OmegaApp : AlphaApp
    {
        public OmegaApp()
        {
        }

        public override string getLanguage()
        {
            return Windows.System.UserProfile.GlobalizationPreferences.Languages[0].Split('-')[0];
        }
    }
}

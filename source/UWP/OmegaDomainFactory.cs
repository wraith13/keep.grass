using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;
using keep.grass.Domain;

namespace keep.grass.UWP
{
    public class OmegaDomainFactory : AlphaDomainFactory
    {
        public static void MakeSureInit()
        {
            if (null == AlphaDomainFactory.Get())
            {
                AlphaDomainFactory.Init(new OmegaDomainFactory());
            }
        }

        public override AlphaDomain MakeOmegaDomain()
        {
            return new OmegaDomain();
        }
        public override AlphaLanguage MakeOmegaLanguage()
        {
            return new OmegaLanguage();
        }
    }
}

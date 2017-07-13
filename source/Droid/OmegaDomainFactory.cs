using System;
using keep.grass.Domain;

namespace keep.grass.Droid
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


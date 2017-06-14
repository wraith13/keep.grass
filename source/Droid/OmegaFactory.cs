﻿using System;
using keep.grass.App;
using keep.grass.Domain;

namespace keep.grass.Droid
{
    public class OmegaFactory : AlphaFactory
    {
        public static void MakeSureInit()
        {
            if (null == AlphaFactory.Get())
            {
                AlphaFactory.Init(new OmegaFactory());
            }
        }

        public override AlphaDomain MakeOmegaDomain()
        {
            return new OmegaDomain();
        }
        public override AlphaApp MakeOmegaApp()
        {
            return new OmegaApp();
        }
        public override AlphaLanguage MakeOmegaLanguage()
        {
            return new OmegaLanguage();
        }
    }
}


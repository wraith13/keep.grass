using System;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;

namespace keep.grass.Domain
{
    public abstract class AlphaDomainFactory
    {
        static AlphaDomainFactory Instance = null;
        AlphaDomain Domain = null;
        AlphaLanguage Language = null;

        public AlphaDomainFactory()
        {
        }
        protected static void Init(AlphaDomainFactory Factory)
        {
            Instance = Factory;
        }
        public static AlphaDomainFactory Get()
        {
            return Instance;
        }

        public static AlphaDomain MakeSureDomain()
        {
            return Instance.Domain ??
                (Instance.Domain = Instance.MakeOmegaDomain());
        }
        public abstract AlphaDomain MakeOmegaDomain();

        public static AlphaLanguage MakeSureLanguage()
        {
            return Instance.Language ??
                (Instance.Language = Instance.MakeOmegaLanguage());
        }
        public virtual AlphaLanguage MakeOmegaLanguage()
        {
            return new AlphaLanguage();
        }

        public static AlphaDrawer MakeDrawer()
        {
            return Instance.MakeOmegaDrawer();
        }
        public virtual AlphaDrawer MakeOmegaDrawer()
        {
            return new AlphaDrawer();
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace keep.grass.UWP
{
    public class OmegaFactory : AlphaFactory
    {
        public new static void Init()
        {
            AlphaFactory.Init(new OmegaFactory());
        }

        public override AlphaApp MakeOmegaApp()
        {
            return new OmegaApp();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

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
        public override ContentPage MakeOmegaSettingsPage(AlphaApp Root)
        {
            return new OmegaSettingsPage(Root);
        }
        public override AlphaActivityIndicatorTextCell MakeOmegaActivityIndicatorTextCell()
        {
            return new OmegaActivityIndicatorTextCell();
        }
        public override AlphaCircleImageCell MakeOmegaCircleImageCell()
        {
            return new OmegaCircleImageCell();
        }
    }
}

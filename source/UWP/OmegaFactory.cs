using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using keep.grass.Languages;
using Xamarin.Forms;

namespace keep.grass.UWP
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
        public override ContentPage MakeOmegaSettingsPage()
        {
            return new OmegaSettingsPage();
        }
        public override AlphaActivityIndicatorTextCell MakeOmegaActivityIndicatorTextCell()
        {
            return new OmegaActivityIndicatorTextCell();
        }
        public override AlphaCircleImageCell MakeOmegaCircleImageCell()
        {
            return new OmegaCircleImageCell();
        }
        public override Image MakeOmegaCircleImage()
        {
            return new Image();
        }
    }
}

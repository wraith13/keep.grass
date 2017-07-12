using System;
using Xamarin.Forms;
using keep.grass.App;

namespace keep.grass.Mac
{
    public class OmegaAppFactory : AlphaAppFactory
    {
        public static void MakeSureInit()
        {
            if (null == AlphaAppFactory.Get())
            {
                AlphaAppFactory.Init(new OmegaAppFactory());
            }
        }
        public override AlphaApp MakeOmegaApp()
        {
            return new OmegaApp();
        }
        /*
        public override AlphaPickerCell MakeOmegaPickerCell()
        {
            return new OmegaPickerCell();
        }
        */
        public override Image MakeOmegaCircleImage()
        {
            return new Image();
        }
    }
}


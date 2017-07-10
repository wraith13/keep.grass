using System;
using keep.grass.App;
using keep.grass.Domain;

namespace keep.grass.iOS
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
        public override AlphaPickerCell MakeOmegaPickerCell()
        {
            return new OmegaPickerCell();
        }
    }
}


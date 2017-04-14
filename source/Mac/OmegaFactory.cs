using System;
namespace keep.grass.Mac
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
        /*
        public override AlphaApp MakeOmegaApp()
        {
            return new OmegaApp();
        }
        public override Languages.AlphaLanguage MakeOmegaLanguage()
        {
            return new OmegaLanguage();
        }
        public override AlphaPickerCell MakeOmegaPickerCell()
        {
            return new OmegaPickerCell();
        }
        */
    }
}


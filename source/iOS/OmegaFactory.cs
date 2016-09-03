using System;
namespace keep.grass.iOS
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
		public override Languages.AlphaLanguage MakeOmegaLanguage()
		{
			return new OmegaLanguage();
		}
		public override AlphaPickerCell MakeOmegaPickerCell()
		{
			return new OmegaPickerCell();
		}
	}
}


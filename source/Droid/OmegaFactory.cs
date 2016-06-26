using System;
namespace keep.grass.Droid
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


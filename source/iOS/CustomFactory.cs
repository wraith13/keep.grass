using System;
namespace keep.grass.iOS
{
	public class CustomFactory : AlphaFactory
	{
		public new static void Init()
		{
			AlphaFactory.Init(new CustomFactory());
		}

		public override AlphaActivityIndicatorTextCell makeCustomActivityIndicatorTextCell()
		{
			return new CustomActivityIndicatorTextCell();
		}
	}
}


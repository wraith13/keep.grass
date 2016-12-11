using System;
using Xamarin.Forms;

namespace keep.grass
{
	public class AlphaUserCircleGraph :AlphaCircleGraph
	{
		AlphaDomain Domain = AlphaFactory.MakeSureDomain();

		public string User
		{
			get
			{
				return AltText;
			}
			set
			{
				var Trimed = value?.Trim();
				if (AltText != Trimed)
				{
					Image = null;
					AltText = Trimed;
					AltTextColor = Color.Black;
					AlphaFactory.MakeImageSourceFromUrl(GitHub.GetIconUrl(User))
						.ContinueWith
						(
							t => Device.BeginInvokeOnMainThread
							(
								() =>
								{
									Image = AlphaImageProxy.GetFromCache(GitHub.GetIconUrl(User));
									AsView().Animate
									(
										"ImageAnimation",
										d => ImageAlpha = (byte)d,
										0.0,
										255.0,
										16,
										1000,
										Easing.SinIn
									);


								}
						   )
					   );
				}
			}
		}
		public AlphaUserCircleGraph()
		{
		}
	}
}

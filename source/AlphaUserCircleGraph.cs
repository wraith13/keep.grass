using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace keep.grass
{
	public abstract class VoidUserCircleGraph : AlphaCircleGraph // プロパティのフィールドを明示的に指定するの避ける為だけのクラス
	{
		public virtual DateTime Now { get; set; }
		public virtual DateTime LastPublicActivity { get; set; }
	}
	public class AlphaUserCircleGraph :VoidUserCircleGraph
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
		public void UpdateSlices()
		{
			SetStartAngle(AlphaDomain.TimeToAngle(Now));
			if (default(DateTime) != LastPublicActivity)
			{
				var Today = Now.Date;
				var LimitTime = LastPublicActivity.AddHours(24);
				var LeftTime = LimitTime - Now;
				var LeftTimeColor = AlphaDomain.MakeLeftTimeColor(LeftTime);

				AltTextColor = LeftTimeColor;

				Data = AlphaDomain.MakeSlices(LeftTime, LeftTimeColor);

				if (GraphSize < FontSize * 9.0f)
				{
					SatelliteTexts = null;
				}
				else
				if (GraphSize < FontSize * 12.0f)
				{
					SatelliteTexts = AlphaDomain.MakeSatelliteTexts(Now, LastPublicActivity, 6);
				}
				else
				if (GraphSize < FontSize * 16.0f)
				{
					SatelliteTexts = AlphaDomain.MakeSatelliteTexts(Now, LastPublicActivity, 3);
				}
				else
				{
					SatelliteTexts = AlphaDomain.MakeSatelliteTexts(Now, LastPublicActivity);
				}

				//Task.Run(() => Domain.AutoUpdateLastPublicActivityAsync());
			}
			else
			{
				Data = AlphaDomain.MakeSlices(TimeSpan.Zero, Color.Lime);
				SatelliteTexts = Enumerable.Range(0, 24).Select
				(
					i => new CircleGraphSatelliteText
					{
						Text = i.ToString(),
						Color = Color.Gray,
						Angle = 360.0f * ((float)(i) / 24.0f),
					}
				);
			}
		}
	}
}

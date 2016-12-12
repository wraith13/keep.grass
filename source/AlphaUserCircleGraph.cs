using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SkiaSharp;
using Xamarin.Forms;

namespace keep.grass
{
	public abstract class VoidUserCircleGraph : AlphaCircleGraph // プロパティのフィールドを明示的に指定するの避ける為だけのクラス
	{
		public virtual bool IsVisible { get; set; }
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

		public override bool IsVisible
		{
			set
			{
				if (base.IsVisible != value)
				{
					base.IsVisible = value;
					if (value)
					{
						OnAnimate();
					}
				}
			}
		}

		private DateTime NewNow;
		public override DateTime Now
		{
			set
			{
				NewNow = value;
				if (IsVisible)
				{
					OnAnimate();
				}
			}
			get
			{
				return new[]
				{
					base.Now,
					DateTime.Now.AddDays(-1),
				}
				.Max();
			}
		}
		private DateTime NewLastPublicActivity;
		public override DateTime LastPublicActivity
		{
			set
			{
				NewLastPublicActivity = value;
				if (IsVisible)
				{
					OnAnimate();
				}
			}
			get
			{
				return new[]
				{
					base.LastPublicActivity,
					DateTime.Now.AddDays(-1),
				}
				.Max();
			}
		}

		public bool AnimationIsRunning()
		{
			return
				null != AsView() &&
				!AsView().AnimationIsRunning("NowAnimation") &&
				!AsView().AnimationIsRunning("LastPublicActivityAnimation");
		}

		public void OnAnimate()
		{
			if (NewLastPublicActivity != LastPublicActivity)
			{
				Debug.WriteLine("Start LastPublicActivity");
				var AnchorNow = DateTime.Now;
				AsView().Animate
				(
					"LastPublicActivityAnimation",
					d =>
					{
						base.LastPublicActivity = AnchorNow.Date + TimeSpan.FromMinutes(d);
						UpdateSlices();
					},
					(LastPublicActivity - AnchorNow.Date).TotalMinutes,
					(NewLastPublicActivity - AnchorNow.Date).TotalMinutes,
					16,
					500,
					Easing.SinOut
				);
			}
			if (Now.AddSeconds(60) < NewNow)
			{
				Debug.WriteLine("Start AnimateNow");
				var AnchorNow = DateTime.Now;
				AsView().Animate
				(
					"NowAnimation",
					d =>
					{
						base.Now = AnchorNow.Date + TimeSpan.FromMinutes(d);
						UpdateSlices();
					},
					(Now - AnchorNow.Date).TotalMinutes,
					(NewNow.AddMilliseconds(500) - AnchorNow.Date).TotalMinutes,
					16,
					500,
					Easing.SinOut
				);
			}
			else
			{
				base.Now = NewNow;
				UpdateSlices();
			}
		}

		public AlphaUserCircleGraph()
		{
		}

		public override void Draw(SKCanvas Canvas)
		{
			IsVisible = true;
			base.Draw(Canvas);
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

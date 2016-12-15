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
		public virtual bool IsVisibleLeftTimeBar { get; set; }
	}
	public class AlphaUserCircleGraph :VoidUserCircleGraph
	{
		Languages.AlphaLanguage L = AlphaFactory.MakeSureLanguage();
		//AlphaDomain Domain = AlphaFactory.MakeSureDomain();

		public TimeSpan AnimationSpan = TimeSpan.FromMilliseconds(1000);

		public override string AltText
		{
			get
			{
				return base.AltText ?? L["unspecified"];
			}
		}
		public override Color AltTextColor
		{
			get
			{
				return null == base.AltText ?
					Color.Gray:
					base.AltTextColor;
			}
		}

		public string User
		{
			get
			{
				return AltText;
			}
			set
			{
				var Trimed = value?.Trim();
				if (base.AltText != Trimed)
				{
					Image = null;
					AltText = Trimed;
					AltTextColor = Color.Black;
					if (!string.IsNullOrWhiteSpace(Trimed))
					{
						AlphaFactory.MakeImageSourceFromUrl(GitHub.GetIconUrl(Trimed))
							.ContinueWith
							(
								t => Device.BeginInvokeOnMainThread
								(
									() =>
									{
										Image = AlphaImageProxy.GetFromCache(GitHub.GetIconUrl(Trimed));
										CanvasAsView().Animate
										(
											"ImageAnimation",
											d => ImageAlpha = (byte)d,
											0.0,
											255.0,
											16,
											(uint)AnimationSpan.TotalMilliseconds,
											Easing.SinIn
										);
									}
							   )
						   );
					}
				}
			}
		}

		public DateTime VisibleAt;
		public TimeSpan VisibleWait = TimeSpan.FromMilliseconds(500);
		public bool IsEarlyVisible => DateTime.Now < VisibleAt.Add(VisibleWait);
		public override bool IsVisible
		{
			set
			{
				if (base.IsVisible != value)
				{
					base.IsVisible = value;
					if (value)
					{
						VisibleAt = DateTime.Now;
						Task.Delay(VisibleWait.Add(TimeSpan.FromMilliseconds(100)))
							.ContinueWith
							(
								t => Device.BeginInvokeOnMainThread
							    (
								    () => StartAnimation()
							   	)
						   	);
					}
				}
			}
		}

		private bool HasNextAnimation;

		public void RequestToAnimation()
		{
			if (IsVisible)
			{
				if (!IsEarlyVisible && !AnimationIsRunning)
				{
					HasNextAnimation = false;
					StartAnimation();
				}
				else
				{
					HasNextAnimation = true;
				}
			}
		}
		private DateTime NewNow;
		public override DateTime Now
		{
			set
			{
				NewNow = value;
				RequestToAnimation();
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
				RequestToAnimation();
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
		public TimeSpan LeftTime => base.LastPublicActivity.AddHours(24) - base.Now;

		public bool AnimationIsRunning =>
				null != AsView() &&
				(
					AsView().AnimationIsRunning("NowAnimation") ||
					AsView().AnimationIsRunning("LastPublicActivityAnimation")
				);

		public void StartAnimation()
		{
			var Delta = TimeSpan.FromSeconds(60);
			if (NewLastPublicActivity != LastPublicActivity)
			{
				if (Delta < (NewLastPublicActivity - LastPublicActivity))
				{
					Debug.WriteLine("Start LastPublicActivity");
					var AnchorNow = DateTime.Now;
					CanvasAsView().Animate
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
						(uint)AnimationSpan.TotalMilliseconds,
						Easing.SinOut
					);
				}
				else
				{
					base.LastPublicActivity = NewLastPublicActivity;
				}
			}
			if (NewNow != Now)
			{
				if (Delta < (NewNow - Now))
				{
					Debug.WriteLine("Start AnimateNow");
					var AnchorNow = DateTime.Now;
					CanvasAsView().Animate
					(
						"NowAnimation",
						d =>
						{
							base.Now = AnchorNow.Date + TimeSpan.FromMinutes(d);
							UpdateSlices();
						},
						(Now - AnchorNow.Date).TotalMinutes,
						(NewNow.AddMilliseconds(1000) - AnchorNow.Date).TotalMinutes,
						16,
						(uint)AnimationSpan.TotalMilliseconds,
						Easing.SinOut
					);
				}
				else
				{
					base.Now = NewNow;
				}
			}

			UpdateSlices();
		}

		public AlphaUserCircleGraph()
		{
		}

		public override void Draw(SKCanvas Canvas)
		{
			IsVisible = true;
			if (HasNextAnimation)
			{
				RequestToAnimation();
			}
			base.Draw(Canvas);
		}

		public void UpdateSlices()
		{
			SetStartAngle(AlphaDomain.TimeToAngle(Now));
			if (default(DateTime) != LastPublicActivity)
			{
				var Today = Now.Date;
				var LeftTimeColor = AlphaDomain.MakeLeftTimeColor(LeftTime);
				AltTextColor = LeftTimeColor;
				Data = AlphaDomain.MakeSlices(LeftTime, LeftTimeColor);

				if (GraphSize < FontSize * 6.0f)
				{
					SatelliteTexts = null;
				}
				else
				if (GraphSize < FontSize * 9.0f)
				{
					SatelliteTexts = AlphaDomain.MakeSatelliteTexts(Now, LastPublicActivity, 6);
				}
				else
				if (GraphSize < FontSize * 12.0f)
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
				AltTextColor = Color.Gray;
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

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
		public virtual bool IsActive { get; set; }
		public virtual DateTime Now { get; set; }
		public virtual DateTime LastPublicActivity { get; set; }
		public virtual bool IsVisibleLeftTimeBar { get; set; }
		public virtual bool IsVisibleSatelliteTexts { get; set; }
	}
	public class AlphaUserCircleGraph :VoidUserCircleGraph
	{
		Languages.AlphaLanguage L = AlphaFactory.MakeSureLanguage();
		AlphaDomain Domain = AlphaFactory.MakeSureDomain();

		public TimeSpan AnimationSpan = TimeSpan.FromMilliseconds(500);

		float LeftTimeBarHeight => FontSize * 2.0f;

		public override bool IsVisibleLeftTimeBar
		{
			set
			{
				if (base.IsVisibleLeftTimeBar != value)
				{
					base.IsVisibleLeftTimeBar = value;
					CircleMargin.Top += value ? LeftTimeBarHeight : -LeftTimeBarHeight;
				}
			}
		}

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
						Image = AlphaImageProxy.GetFromCache(GitHub.GetIconUrl(Trimed));
					}
				}
			}
		}
		public override byte[] Image
		{
			set
			{
				if (base.Image != value)
				{
					var HasRequestToAnimation = null == base.Image;
					base.Image = value;
					if (HasRequestToAnimation)
					{
						if (IsActive)
						{
							StartIconAnimation();
						}
						else
						{
							ImageAlpha = 0;
						}
					}
				}
			}
		}
		public void StartIconAnimation()
		{
			this.Animate
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

		public DateTime ActiveAt;
		public TimeSpan ActiveWait = TimeSpan.FromMilliseconds(100);
		public bool IsEarlyActive => DateTime.Now < ActiveAt.Add(ActiveWait);
		public override bool IsActive
		{
			set
			{
				if (base.IsActive != value)
				{
					base.IsActive = value;
					if (value)
					{
						ActiveAt = DateTime.Now;
						Task.Delay(ActiveWait.Add(TimeSpan.FromMilliseconds(100)))
							.ContinueWith
							(
								t => Device.BeginInvokeOnMainThread
								(
									() =>
									{
										StartAnimation();
										StartIconAnimation();
									}
								)
							);
					}
				}
			}
		}

		private bool HasNextAnimation;

		public void RequestToAnimation()
		{
			if (IsActive)
			{
				if (!IsEarlyActive && !AnimationIsRunning)
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

		public void ResetTime()
		{
			base.Now = default(DateTime);
			base.LastPublicActivity = default(DateTime);
			ImageAlpha = 0;
			UpdateSlices();
		}

		public bool AnimationIsRunning =>
					((View)this).AnimationIsRunning("NowAnimation") ||
					((View)this).AnimationIsRunning("LastPublicActivityAnimation");

		public void StartAnimation()
		{
			var Delta = TimeSpan.FromSeconds(60);
			if (NewLastPublicActivity != LastPublicActivity)
			{
				if (Delta < (NewLastPublicActivity - LastPublicActivity))
				{
					Debug.WriteLine("Start LastPublicActivity");
					var AnchorNow = DateTime.Now;
					this.Animate
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
					this.Animate
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
			IsActive = true;
			if (HasNextAnimation)
			{
				RequestToAnimation();
			}
			var IsInvalidBar = IsInvalidData;
			base.Draw(Canvas);
			if (IsVisibleLeftTimeBar && IsInvalidBar)
			{
				DrawLeftTimeBar(Canvas);
			}
		}
		public override void ClearCanvas(SKCanvas Canvas)
		{
			base.ClearCanvas(Canvas);
			UpdateSlices();
		}
		private void DrawLeftTimeBar(SKCanvas Canvas)
		{
			var LeftTimeBarRect = new SKRect
			(
				CanvasRect.Left,
				CanvasRect.Top,
				CanvasRect.Right,
				CanvasRect.Top + (LeftTimeBarHeight * PhysicalPixelRate)
			);
			using (var paint = new SKPaint())
			{
				if (default(DateTime) != base.LastPublicActivity)
				{
					paint.Color = AlphaDomain.MakeLeftTimeColor(LeftTime).ToSKColor();
				}
				else
				{
					paint.Color = AlphaDomain.GetElapsedTimeColor().ToSKColor();
				}
				Canvas.DrawRect
				(
					LeftTimeBarRect,
					paint
				);
			}
			if (default(DateTime) != base.LastPublicActivity)
			{
				using (var paint = new SKPaint())
				{
					paint.IsAntialias = true;
					paint.Color = Color.White.ToSKColor();
					paint.StrokeCap = SKStrokeCap.Round;
					paint.TextSize = FontSize * PhysicalPixelRate;
					paint.TextAlign = SKTextAlign.Center;
					paint.Typeface = Font;

					Canvas.DrawText
					(
						L["Left Time"] +" : " +Domain.ToString(LeftTime),
						LeftTimeBarRect.MidX,
						LeftTimeBarRect.MidY + (paint.TextSize / 2.0f),
						paint
					);

					paint.Typeface = null;
				}
			}
		}

		public void UpdateSlices()
		{
			SetStartAngle(AlphaDomain.TimeToAngle(Now));
			if (default(DateTime) != base.LastPublicActivity)
			{
				var Today = Now.Date;
				var LeftTimeColor = AlphaDomain.MakeLeftTimeColor(LeftTime);
				AltTextColor = LeftTimeColor;
				Data = AlphaDomain.MakeSlices(LeftTime, LeftTimeColor);

				if (!IsVisibleSatelliteTexts || GraphSize < FontSize * 6.0f)
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
				if (!IsVisibleSatelliteTexts)
				{
					SatelliteTexts = null;
				}
				else
				{
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
}

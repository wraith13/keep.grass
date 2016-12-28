using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using System.Threading.Tasks;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Diagnostics;

namespace keep.grass
{
	public interface VoidPie
	{
		string Text { get; }
		double Volume { get; }
		Color Color { get; }
		string DisplayVolume { get; }
	}
	public class CircleGraphSatelliteText
	{
		public string Text { get; set; }
		public Color Color { get; set; }
		public float Angle { get; set; }
	}
	public class NumberPie : VoidPie
	{
		public string Text { get; set; }
		public double Value { get; set; }
		public double Volume { get { return Value; } }
		public Color Color { get; set; }
		public string DisplayVolume { get { return Value.ToString(); } }
	}
	public class TimePie : VoidPie
	{
		public string Text { get; set; }
		public TimeSpan Value { get; set; }
		public double Volume { get { return Value.Ticks; } }
		public Color Color { get; set; }
		public string DisplayVolume { get { return TimeToString(Value); } }
		public static string TimeToString(TimeSpan a)
		{
			return Math.Floor(a.TotalHours).ToString() + a.ToString("\\:mm\\:ss");
		}
	}
	public abstract class VoidCircleGraph :SKCanvasView // プロパティのフィールドを明示的に指定するの避ける為だけのクラス
	{
		public virtual bool IsDoughnut { get; set; }
		public virtual string AltText { get; set; }
		public virtual Color AltTextColor { get; set; }
		public virtual bool IsInvalidCanvas { get; set; }
		public virtual bool IsInvalidCenter { get; set; }
		public virtual bool IsInvalidSatelliteTexts { get; set; }
		public virtual bool IsInvalidData { get; set; }
		public virtual bool IsClearCanvas { get; set; }
		public virtual byte[] Image { get; set; }
		public virtual byte ImageAlpha { get; set; }
		public virtual IEnumerable<VoidPie> Data { get; set; }
		public virtual IEnumerable<CircleGraphSatelliteText> SatelliteTexts { get; set; }
	}
	public class AlphaCircleGraph :VoidCircleGraph
	{
		public readonly float Phi = 1.618033988749894848204586834365f;
		float OriginAngle = -90.0f;
		float StartAngle = 0.0f;
		public double GraphSize;
		public float FontSize = 14.0f;
		public Thickness CircleMargin = new Thickness(30.0f);
		float AntialiasMargin = 0.6f;

		public override bool IsDoughnut
		{
			set
			{
				if (base.IsDoughnut != value)
				{
					base.IsDoughnut = value;
					IsInvalidCenter = true;
				}
			}
		}
		public override string AltText
		{
			set
			{
				if (base.AltText != value)
				{
					base.AltText = value;
					IsInvalidCenter = true;
				}
			}
		}
		public override Color AltTextColor
		{
			set
			{
				if (base.AltTextColor != value)
				{
					base.AltTextColor = value;
					IsInvalidCenter = true;
				}
			}
		}
		public override bool IsInvalidCanvas
		{
			set
			{
				if (base.IsInvalidCanvas != value)
				{
					base.IsInvalidCanvas = value;
					if (base.IsInvalidCanvas)
					{
						Update();
					}
				}
			}
		}
		public override bool IsInvalidCenter
		{
			set
			{
				if (base.IsInvalidCenter != value)
				{
					base.IsInvalidCenter = value;
					if (base.IsInvalidCenter)
					{
						Update();
					}
				}
			}
		}

		public override bool IsInvalidSatelliteTexts
		{
			set
			{
				if (base.IsInvalidSatelliteTexts != value)
				{
					base.IsInvalidSatelliteTexts = value;
					if (base.IsInvalidSatelliteTexts)
					{
						Update();
					}
				}
			}
		}

		public override bool IsInvalidData
		{
			set
			{
				if (base.IsInvalidSatelliteTexts != value)
				{
					base.IsInvalidData = value;
					if (base.IsInvalidData)
					{
						Update();
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
					ImageBitmap?.Dispose();
					ImageBitmap = null;
					ImageData?.Dispose();
					ImageData = null;
					base.Image = value;
					if (null != base.Image)
					{
						ImageData = new SKData(base.Image);
						ImageBitmap = SKBitmap.Decode(ImageData);
					}
					IsInvalidCenter = true;
				}
			}
		}
		public override byte ImageAlpha
		{
			set
			{
				if (base.ImageAlpha != value)
				{
					base.ImageAlpha = value;
					if (null != base.Image)
					{
						IsInvalidCenter = true;
					}
				}
			}
		}
		public override IEnumerable<VoidPie> Data
		{
			set
			{
				base.Data = value;
				IsInvalidData = true;
			}
		}
		public override IEnumerable<CircleGraphSatelliteText> SatelliteTexts
		{
			set
			{
				base.SatelliteTexts = value;
				IsInvalidSatelliteTexts = true;
			}
		}

		//Grid GraphFrame;
		//AlphaCircleGraphView CanvasView;

		System.IO.Stream FontSource;
		SKManagedStream FontStream;
		protected SKTypeface Font;

		SKData ImageData;
		SKBitmap ImageBitmap;

		public SKRect CanvasRect;
		public float PhysicalPixelRate;
		float PieRadius;
		float ImageRadius;
		SKPoint Center;

		public AlphaCircleGraph()
		{
			//	※iOS 版では Font だけ残して他はこの場で Dispose() して構わないが Android 版では遅延処理が行われるようでそれだと disposed object へのアクセスが発生してしまう。
			FontSource = AlphaFactory.GetApp().GetFontStream();
			FontStream = new SKManagedStream(FontSource);
			Font = SKTypeface.FromStream(FontStream);
		}
		public void Dispose()
		{
			Font?.Dispose();
			Font = null;
			FontStream?.Dispose();
			FontStream = null;
			FontSource?.Dispose();
			FontSource = null;
			ImageBitmap?.Dispose();
			ImageBitmap = null;
			ImageData?.Dispose();
			ImageData = null;
		}

		public static double DegreeToRadian(double Degree)
		{
			return Degree * Math.PI / 180.0;
		}
		public static SKPoint AngleRadiusToPoint(float Angle, float Radius)
		{
			return new SKPoint
			(
				Radius * (float)Math.Cos(DegreeToRadian(Angle)),
				Radius * (float)Math.Sin(DegreeToRadian(Angle))
			);
		}
		public double GetTotalVolume()
		{
			return Data.Select(Pie => Pie.Volume).Sum();
		}
		private float GetStartAngle()
		{
			return StartAngle +OriginAngle;
		}
		public void SetStartAngle(float NewStartAngle)
		{
			StartAngle = NewStartAngle;
		}
        private void Update()
		{
			InvalidateSurface();
		}
		protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
		{
			Draw(e.Surface.Canvas);
		}
		public virtual void Draw(SKCanvas Canvas)
		{
			if (IsInvalidCanvas)
			{
				ClearCanvas(Canvas);
			}
			if (IsInvalidCenter)
			{
				DrawCenter(Canvas);
			}
			if (IsInvalidSatelliteTexts)
			{
				DrawSatelliteTexts(Canvas);
			}
			if (IsInvalidData)
			{
				DrawData(Canvas);
			}
			IsClearCanvas = false;
		}
		public virtual void ClearCanvas(SKCanvas Canvas)
		{
			GraphSize = new[]
			{
					Width -CircleMargin.HorizontalThickness,
					Height -CircleMargin.VerticalThickness
				}.Min();
			if (FontSize * 15.0f < GraphSize)
			{
				GraphSize = Math.Max
				(
					FontSize * 15.0f,
					Math.Min
					(
						Width / Phi,
						Height / Phi
					)
				);
			}
			Canvas.GetClipBounds(ref CanvasRect);
			PhysicalPixelRate = (float)((CanvasRect.Width + CanvasRect.Height) / (Width + Height));
			var DrawGraphSize = (float)(GraphSize * PhysicalPixelRate);
			PieRadius = DrawGraphSize / 2.0f;
			var CanvasSpaceHeight = (float)((CanvasRect.Height - (CircleMargin.VerticalThickness * PhysicalPixelRate)) - DrawGraphSize);
			Center = new SKPoint
			(
				CanvasRect.MidX + ((float)((CircleMargin.Left - CircleMargin.Right) * PhysicalPixelRate)) / 2.0f,
				//CanvasRect.MidY +((float)((CircleMargin.Top -CircleMargin.Bottom) *PhysicalPixelRate)) / 2.0f
				(float)(CircleMargin.Top * PhysicalPixelRate) + PieRadius + (CanvasSpaceHeight * (1.0f - (1.0f / Phi)))
			);
			ImageRadius = PieRadius / Phi;

			IsInvalidCanvas = false;
			IsInvalidCenter = true;
			IsInvalidSatelliteTexts = true;
			IsInvalidData = true;
			Canvas.Clear();
			IsClearCanvas = true;
		}
		private void DrawCenter(SKCanvas Canvas)
		{
			IsInvalidCenter = false;
			IsInvalidData = true;
			var AntialiasImageRadius = ImageRadius - AntialiasMargin;

			//	背景を塗りつぶす
			if (!IsClearCanvas)
			{
				using (var paint = new SKPaint())
				{
					paint.IsAntialias = true;
					paint.Color = Color.White.ToSKColor();
					paint.StrokeCap = SKStrokeCap.Round;
					Canvas.DrawCircle(Center.X, Center.Y, ImageRadius + AntialiasMargin, paint);
				}
			}

			//	イメージの描画
			if (null != Image && null != ImageData && null != ImageBitmap)
			{
				using (var paint = new SKPaint())
				{
					var Rect = SKRect.Create
					(
						Center.X - AntialiasImageRadius,
						Center.Y - AntialiasImageRadius,
						AntialiasImageRadius * 2.0f,
						AntialiasImageRadius * 2.0f
					);
					Canvas.DrawBitmap(ImageBitmap, Rect, paint);
				}
				if (ImageAlpha < 255)
				{
					using (var paint = new SKPaint())
					{
						paint.IsAntialias = true;
						paint.Color = new SKColor(255, 255, 255, (byte)(255 -ImageAlpha));
						paint.StrokeCap = SKStrokeCap.Round;
						paint.IsStroke = false;
						Canvas.DrawCircle(Center.X, Center.Y, AntialiasImageRadius, paint);
					}
				}
			}
			else
			//	Altテキストの描画
			if (!String.IsNullOrWhiteSpace(AltText))
			{
				using (var paint = new SKPaint())
				{
					paint.IsAntialias = true;
					paint.Color = AltTextColor.ToSKColor();
					paint.StrokeCap = SKStrokeCap.Round;
					paint.TextSize = FontSize * PhysicalPixelRate;
					paint.TextAlign = SKTextAlign.Center;
					paint.Typeface = Font;

					Canvas.DrawText
					(
						AltText,
						Center.X,
						Center.Y + (paint.TextSize / 2.0f),
						paint
					);

					paint.Typeface = null;
				}
			}

			//	輪郭の円の描画 ( これより外側の部分は DrawData() に任せる )
			using (var paint = new SKPaint())
			{
				paint.IsAntialias = true;
				paint.Color = Color.White.ToSKColor();
				paint.StrokeCap = SKStrokeCap.Round;
				paint.IsStroke = true;
				paint.StrokeWidth = PhysicalPixelRate;
				using (var path = new SKPath())
				{
					path.ArcTo(SKRect.Create(Center.X - AntialiasImageRadius, Center.Y - AntialiasImageRadius, AntialiasImageRadius * 2.0f, AntialiasImageRadius * 2.0f), 0.0f, 180.0f, false);
					Canvas.DrawPath(path, paint);
				}
				using (var path = new SKPath())
				{
					path.ArcTo(SKRect.Create(Center.X - AntialiasImageRadius, Center.Y - AntialiasImageRadius, AntialiasImageRadius * 2.0f, AntialiasImageRadius * 2.0f), 180.0f, 180.0f, false);
					Canvas.DrawPath(path, paint);
				}
			}
		}
		private void DrawSatelliteTexts(SKCanvas Canvas)
		{
			IsInvalidSatelliteTexts = false;
			//	周辺テキストの描画
			if (SatelliteTexts?.Any() ?? false)
			{
				if (!IsClearCanvas)
				{
					//	背景をクリア
					var AntialiasPieRadius = PieRadius +AntialiasMargin;
					using (var paint = new SKPaint())
					{
						paint.IsAntialias = true;
						paint.Color = Color.White.ToSKColor();
						paint.StrokeCap = SKStrokeCap.Round;
						using (var path = new SKPath())
						{
							path.MoveTo(CanvasRect.Right, CanvasRect.Top);
							path.LineTo(Center.X, CanvasRect.Top);
							path.LineTo(Center.X, Center.Y - AntialiasPieRadius);
							path.ArcTo(SKRect.Create(Center.X - AntialiasPieRadius, Center.Y - AntialiasPieRadius, AntialiasPieRadius * 2.0f, AntialiasPieRadius * 2.0f), 0.0f +OriginAngle, 180.0f, false);
							path.LineTo(Center.X, CanvasRect.Bottom);
							path.LineTo(CanvasRect.Right, CanvasRect.Bottom);
							path.LineTo(CanvasRect.Right, CanvasRect.Top);
							path.Close();
							Canvas.DrawPath(path, paint);
						}
						using (var path = new SKPath())
						{
							path.MoveTo(CanvasRect.Left, CanvasRect.Top);
							path.LineTo(Center.X, CanvasRect.Top);
							path.LineTo(Center.X, Center.Y - AntialiasPieRadius);
							path.ArcTo(SKRect.Create(Center.X - AntialiasPieRadius, Center.Y - AntialiasPieRadius, AntialiasPieRadius * 2.0f, AntialiasPieRadius * 2.0f), 0.0f +OriginAngle, -180.0f, false);
							path.LineTo(Center.X, CanvasRect.Bottom);
							path.LineTo(CanvasRect.Left, CanvasRect.Bottom);
							path.LineTo(CanvasRect.Left, CanvasRect.Top);
							path.Close();
							Canvas.DrawPath(path, paint);
						}
					}
				}
				
				foreach (var SatelliteText in SatelliteTexts)
				{
					if (!String.IsNullOrWhiteSpace(SatelliteText.Text))
					{
						using (var paint = new SKPaint())
						{
							paint.IsAntialias = true;
							paint.Color = SatelliteText.Color.ToSKColor();
							paint.StrokeCap = SKStrokeCap.Round;
							paint.TextSize = FontSize * PhysicalPixelRate;
							paint.TextAlign = SKTextAlign.Center;
							paint.Typeface = Font;

							var TextRadius = PieRadius + paint.TextSize;
							var TextCenter = Center + AngleRadiusToPoint(SatelliteText.Angle + OriginAngle, TextRadius);

							Canvas.DrawText
							(
								SatelliteText.Text,
								TextCenter.X,
								TextCenter.Y + (paint.TextSize / 2.0f),
								paint
							);

							paint.Typeface = null;
						}
					}
				}
			}
		}
		private void DrawData(SKCanvas Canvas)
		{
			IsInvalidData = false;
			var AntialiasPieRadius = PieRadius - AntialiasMargin;
			var AntialiasImageRadius = ImageRadius + AntialiasMargin;

			//	内側の輪郭の円の描画
			using (var paint = new SKPaint())
			{
				paint.IsAntialias = true;
				paint.Color = Color.White.ToSKColor();
				paint.StrokeCap = SKStrokeCap.Round;
				paint.IsStroke = true;
				paint.StrokeWidth = PhysicalPixelRate;
				using (var path = new SKPath())
				{
					path.ArcTo(SKRect.Create(Center.X - AntialiasImageRadius, Center.Y - AntialiasImageRadius, AntialiasImageRadius * 2.0f, AntialiasImageRadius * 2.0f), 0.0f, 180.0f, false);
					Canvas.DrawPath(path, paint);
				}
				using (var path = new SKPath())
				{
					path.ArcTo(SKRect.Create(Center.X - AntialiasImageRadius, Center.Y - AntialiasImageRadius, AntialiasImageRadius * 2.0f, AntialiasImageRadius * 2.0f), 180.0f, 180.0f, false);
					Canvas.DrawPath(path, paint);
				}
			}

			if (null == Data || !Data.Any())
			{
				using (var paint = new SKPaint())
				{
					paint.IsAntialias = true;
					paint.Color = Color.Gray.ToSKColor();
					paint.StrokeCap = SKStrokeCap.Round;
					if (null != Image || IsDoughnut)
					{
						//	一度に描画しようとしても path が繋がらないので半分に分けて描画する
						using (var path = new SKPath())
						{
							path.MoveTo(Center + AngleRadiusToPoint(0.0f, AntialiasImageRadius));
							path.LineTo(Center + AngleRadiusToPoint(0.0f, AntialiasPieRadius));
							path.ArcTo(SKRect.Create(Center.X - AntialiasPieRadius, Center.Y - AntialiasPieRadius, AntialiasPieRadius * 2.0f, AntialiasPieRadius * 2.0f), 0.0f, 180.0f, false);
							//path.LineTo(Center + AngleRadiusToPoint(180.0f, AntialiasPieRadius));
							path.LineTo(Center + AngleRadiusToPoint(180.0f, ImageRadius));
							path.ArcTo(SKRect.Create(Center.X - AntialiasImageRadius, Center.Y - AntialiasImageRadius, AntialiasImageRadius * 2.0f, AntialiasImageRadius * 2.0f), 180.0f, -180.0f, false);
							path.Close();
							Canvas.DrawPath(path, paint);
						}
						using (var path = new SKPath())
						{
							path.MoveTo(Center + AngleRadiusToPoint(180.0f, AntialiasImageRadius));
							path.LineTo(Center + AngleRadiusToPoint(180.0f, AntialiasPieRadius));
							path.ArcTo(SKRect.Create(Center.X - AntialiasPieRadius, Center.Y - AntialiasPieRadius, AntialiasPieRadius * 2.0f, AntialiasPieRadius * 2.0f), 180.0f, 180.0f, false);
							//path.LineTo(Center + AngleRadiusToPoint(180.0f, AntialiasPieRadius));
							path.LineTo(Center + AngleRadiusToPoint(360.0f, AntialiasImageRadius));
							path.ArcTo(SKRect.Create(Center.X - AntialiasImageRadius, Center.Y - AntialiasImageRadius, AntialiasImageRadius * 2.0f, AntialiasImageRadius * 2.0f), 360.0f, -180.0f, false);
							path.Close();
							Canvas.DrawPath(path, paint);
						}
					}
					else
					{
						Canvas.DrawCircle(Center.X, Center.Y, AntialiasPieRadius, paint);
					}
				}
			}
			else
			{
				//	パイ本体の描画
				var TotalVolume = GetTotalVolume();
				var CurrentAngle = GetStartAngle();
				foreach (var Pie in Data)
				{
					var CurrentAngleVolume = (float)((Pie.Volume / TotalVolume) * 360.0);
					var NextAngle = CurrentAngle + CurrentAngleVolume;
					using (var paint = new SKPaint())
					{
						paint.IsAntialias = true;
						paint.Color = Pie.Color.ToSKColor();
						paint.StrokeCap = SKStrokeCap.Round;
						if (Pie.Volume < TotalVolume)
						{
							using (var path = new SKPath())
							{
								if (null != Image || IsDoughnut)
								{
									path.MoveTo(Center + AngleRadiusToPoint(CurrentAngle, AntialiasImageRadius));
									path.LineTo(Center + AngleRadiusToPoint(CurrentAngle, AntialiasPieRadius));
									path.ArcTo(SKRect.Create(Center.X - AntialiasPieRadius, Center.Y - AntialiasPieRadius, AntialiasPieRadius * 2.0f, AntialiasPieRadius * 2.0f), CurrentAngle, CurrentAngleVolume, false);
									//path.LineTo(Center + AngleRadiusToPoint(NextAngle, AntialiasPieRadius));
									path.LineTo(Center + AngleRadiusToPoint(NextAngle, AntialiasImageRadius));
									path.ArcTo(SKRect.Create(Center.X - AntialiasImageRadius, Center.Y - AntialiasImageRadius, AntialiasImageRadius * 2.0f, AntialiasImageRadius * 2.0f), NextAngle, -CurrentAngleVolume, false);
								}
								else
								{
									path.MoveTo(Center);
									path.LineTo(Center + AngleRadiusToPoint(CurrentAngle, AntialiasPieRadius));
									path.ArcTo(SKRect.Create(Center.X - AntialiasPieRadius, Center.Y - AntialiasPieRadius, AntialiasPieRadius * 2.0f, AntialiasPieRadius * 2.0f), CurrentAngle, CurrentAngleVolume, false);
									path.LineTo(Center + AngleRadiusToPoint(NextAngle, AntialiasPieRadius));
									path.LineTo(Center);
								}
								path.Close();
								Canvas.DrawPath(path, paint);
							}
						}
						else
						{
							//	TotalVolume <= Pie.Volume な時に上の処理ではパイが描画されないことがある。
							if (null != Image || IsDoughnut)
							{
								//	一度に描画しようとしても path が繋がらないので半分に分けて描画する
								using (var path = new SKPath())
								{
									path.MoveTo(Center + AngleRadiusToPoint(0.0f, AntialiasImageRadius));
									path.LineTo(Center + AngleRadiusToPoint(0.0f, AntialiasPieRadius));
									path.ArcTo(SKRect.Create(Center.X - AntialiasPieRadius, Center.Y - AntialiasPieRadius, AntialiasPieRadius * 2.0f, AntialiasPieRadius * 2.0f), 0.0f, 180.0f, false);
									//path.LineTo(Center + AngleRadiusToPoint(180.0f, AntialiasPieRadius));
									path.LineTo(Center + AngleRadiusToPoint(180.0f, AntialiasImageRadius));
									path.ArcTo(SKRect.Create(Center.X - AntialiasImageRadius, Center.Y - AntialiasImageRadius, AntialiasImageRadius * 2.0f, AntialiasImageRadius * 2.0f), 180.0f, -180.0f, false);
									path.Close();
									Canvas.DrawPath(path, paint);
								}
								using (var path = new SKPath())
								{
									path.MoveTo(Center + AngleRadiusToPoint(180.0f, AntialiasImageRadius));
									path.LineTo(Center + AngleRadiusToPoint(180.0f, AntialiasPieRadius));
									path.ArcTo(SKRect.Create(Center.X - AntialiasPieRadius, Center.Y - AntialiasPieRadius, AntialiasPieRadius * 2.0f, AntialiasPieRadius * 2.0f), 180.0f, 180.0f, false);
									//path.LineTo(Center + AngleRadiusToPoint(180.0f, AntialiasPieRadius));
									path.LineTo(Center + AngleRadiusToPoint(360.0f, AntialiasImageRadius));
									path.ArcTo(SKRect.Create(Center.X - AntialiasImageRadius, Center.Y - AntialiasImageRadius, AntialiasImageRadius * 2.0f, AntialiasImageRadius * 2.0f), 360.0f, -180.0f, false);
									path.Close();
									Canvas.DrawPath(path, paint);
								}
							}
							else
							{
								Canvas.DrawCircle(Center.X, Center.Y, AntialiasPieRadius, paint);
							}
						}
					}
					CurrentAngle = NextAngle;
				}

				//	セパレーターの描画
				CurrentAngle = GetStartAngle();
				foreach (var Pie in Data)
				{
					var CurrentAngleVolume = (float)((Pie.Volume / TotalVolume) * 360.0);
					var NextAngle = CurrentAngle + CurrentAngleVolume;
					using (var paint = new SKPaint())
					{
						paint.IsAntialias = true;
						paint.Color = Color.White.ToSKColor();
						paint.StrokeCap = SKStrokeCap.Round;
						paint.IsStroke = true;
						paint.StrokeWidth = PhysicalPixelRate;
						using (var path = new SKPath())
						{
							if (null != Image || IsDoughnut)
							{
								//path.ArcTo(SKRect.Create(Center.X - ImageRadius, Center.Y - ImageRadius, ImageRadius * 2.0f, ImageRadius * 2.0f), CurrentAngle, CurrentAngleVolume, false);
								path.MoveTo(Center + AngleRadiusToPoint(CurrentAngle, AntialiasImageRadius));
							}
							else
							{
								path.MoveTo(Center);
							}
							path.LineTo(Center + AngleRadiusToPoint(CurrentAngle, AntialiasPieRadius));
							Canvas.DrawPath(path, paint);
						}
					}
					CurrentAngle = NextAngle;
				}

				//	パイ・テキストの描画
				if (null == Image && !IsDoughnut)
				{
					foreach (var Pie in Data)
					{
						var CurrentAngleVolume = (float)((Pie.Volume / TotalVolume) * 360.0);
						var NextAngle = CurrentAngle + CurrentAngleVolume;
						if (!String.IsNullOrWhiteSpace(Pie.Text) || !String.IsNullOrWhiteSpace(Pie.DisplayVolume))
						{
							using (var paint = new SKPaint())
							{
								paint.IsAntialias = true;
								paint.StrokeCap = SKStrokeCap.Round;
								using (var path = new SKPath())
								{
									paint.TextSize = FontSize * PhysicalPixelRate;
									paint.IsAntialias = true;
									paint.Color = Color.White.ToSKColor();
									paint.TextAlign = SKTextAlign.Center;
									paint.Typeface = Font;

									var CenterAngle = (CurrentAngle + NextAngle) / 2.0f;
									var HalfRadius = AntialiasPieRadius / 2.0f;
									var TextCenter = Center + AngleRadiusToPoint(CenterAngle, HalfRadius);

									if (!String.IsNullOrWhiteSpace(Pie.Text))
									{
										Canvas.DrawText
										(
											Pie.Text,
											TextCenter.X,
											TextCenter.Y - (paint.TextSize / 2.0f),
											paint
										);
									}
									if (!String.IsNullOrWhiteSpace(Pie.DisplayVolume))
									{
										Canvas.DrawText
										(
											Pie.DisplayVolume,
											TextCenter.X,
											TextCenter.Y + (paint.TextSize / 2.0f),
											paint
										);
									}

									paint.Typeface = null;
								}
							}
						}
						CurrentAngle = NextAngle;
					}
				}
			}

			//	外側の輪郭の円の描画
			using (var paint = new SKPaint())
			{
				paint.IsAntialias = true;
				paint.Color = Color.White.ToSKColor();
				paint.StrokeCap = SKStrokeCap.Round;
				paint.IsStroke = true;
				paint.StrokeWidth = PhysicalPixelRate;
				using (var path = new SKPath())
				{
					path.ArcTo(SKRect.Create(Center.X - AntialiasPieRadius, Center.Y - AntialiasPieRadius, AntialiasPieRadius * 2.0f, AntialiasPieRadius * 2.0f), 0.0f, 180.0f, false);
					Canvas.DrawPath(path, paint);
				}
				using (var path = new SKPath())
				{
					path.ArcTo(SKRect.Create(Center.X - AntialiasPieRadius, Center.Y - AntialiasPieRadius, AntialiasPieRadius * 2.0f, AntialiasPieRadius * 2.0f), 180.0f, 180.0f, false);
					Canvas.DrawPath(path, paint);
				}
			}
		}
	}
}

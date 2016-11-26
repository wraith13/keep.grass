using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using System.Threading.Tasks;

using SkiaSharp;
using SkiaSharp.Views.Forms;

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
	public static class SkiaUtil
	{
		public static void MoveTo(this SKPath Path, SKPoint Point)
		{
			Path.MoveTo(Point.X, Point.Y);
		}
		public static void LineTo(this SKPath Path, SKPoint Point)
		{
			Path.LineTo(Point.X, Point.Y);
		}
	}
	public class AlphaCircleGraph :IDisposable
	{
		float OriginAngle = -90.0f;
		float StartAngle = 0.0f;
		double GraphSize;
		float Margin = 30.0f;
		Grid GraphFrame;
		AlphaCircleGraphView CanvasView;

		System.IO.Stream FontSource;
		SKManagedStream FontStream;
		SKTypeface Font;

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
		}

		public void Build(double Width, double Height)
		{
			var MinSide = new[] { Width, Height }.Min();
			GraphSize = Math.Round((MinSide * 0.6) +(Margin *2.0f));
			CanvasView = new AlphaCircleGraphView(this)
			{
				Margin = new Thickness(0.0),
				WidthRequest = GraphSize,
				HeightRequest = GraphSize,
				BackgroundColor = Color.White,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
			};
			var PaddingSize = new[] { (MinSide - 640) * 0.3, 0 }.Max();
			GraphFrame = new Grid()
			{
				MinimumWidthRequest = GraphSize + (PaddingSize *2.0),
				MinimumHeightRequest = GraphSize + (PaddingSize * 2.0),
				BackgroundColor = Color.White,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
			}
			.SetSingleChild(CanvasView);
			Update();
		}

		public byte[] Image { get; set; }
		public IEnumerable<VoidPie> Data { get; set; }
		public IEnumerable<CircleGraphSatelliteText> SatelliteTexts { get; set; }

		public static SKColor ToSKColor(Color c)
		{
			return new SKColor
			(
				(byte)(c.R *255),
				(byte)(c.G *255),
				(byte)(c.B *255),
				(byte)(c.A *255)
			);
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
        public virtual SKColorType GetDeviceColorType()
        {
            return SKColorType.Rgba8888;
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
        public void Update()
		{
			if (null != CanvasView)
			{
				CanvasView.InvalidateSurface();
			}
		}

		public void Draw(SKCanvas canvas)
		{
			canvas.Clear();
			SKRect rect;
			canvas.GetClipBounds(ref rect);
			var Phi = 1.618033988749894848204586834365f;
			var PhysicalPixelRate = (float)((rect.Width + rect.Height) / (CanvasView.Width + CanvasView.Height));
			var DrawGraphSize = (float)(GraphSize * PhysicalPixelRate);
			var Radius = (DrawGraphSize / 2.0f) - (Margin * PhysicalPixelRate);
			var Center = new SKPoint(DrawGraphSize / 2.0f, DrawGraphSize / 2.0f);

			if (null == Data || !Data.Any())
			{
				using (var paint = new SKPaint())
				{
					paint.IsAntialias = true;
					paint.Color = ToSKColor(Color.Gray);
					paint.StrokeCap = SKStrokeCap.Round;
					using (var path = new SKPath())
					{
						path.AddCircle(Center.X, Center.Y, Radius);
						path.Close();
						canvas.DrawPath(path, paint);
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
						paint.Color = ToSKColor(Pie.Color);
						paint.StrokeCap = SKStrokeCap.Round;
						using (var path = new SKPath())
						{
							if (Pie.Volume < TotalVolume)
							{
								path.AddArc(SKRect.Create(Center.X - Radius, Center.Y - Radius, Radius * 2.0f, Radius * 2.0f), CurrentAngle, CurrentAngleVolume);
								path.MoveTo(Center);
								path.LineTo(Center + AngleRadiusToPoint(CurrentAngle, Radius));
								path.LineTo(Center + AngleRadiusToPoint(NextAngle, Radius));
								path.LineTo(Center);
								path.Close();
								canvas.DrawPath(path, paint);
							}
							else
							{
								//	TotalVolume <= Pie.Volume な時に上の処理ではパイが描画されないことがある。
								canvas.DrawCircle(Center.X, Center.Y, Radius, paint);
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
						paint.Color = ToSKColor(Color.White);
						paint.StrokeCap = SKStrokeCap.Round;
						paint.IsStroke = true;
						paint.StrokeWidth = PhysicalPixelRate;
						using (var path = new SKPath())
						{
							path.MoveTo(Center);
							path.LineTo(Center + AngleRadiusToPoint(CurrentAngle, Radius));
							path.Close();
							canvas.DrawPath(path, paint);
						}
					}
					CurrentAngle = NextAngle;
				}

				CurrentAngle = GetStartAngle();
				//	周辺テキストの描画
				if (null != SatelliteTexts)
				{
					foreach (var SatelliteText in SatelliteTexts)
					{
						if (!String.IsNullOrWhiteSpace(SatelliteText.Text))
						{
							using (var paint = new SKPaint())
							{
								paint.IsAntialias = true;
								paint.Color = ToSKColor(SatelliteText.Color);
								paint.StrokeCap = SKStrokeCap.Round;
								paint.TextSize = 14.0f * PhysicalPixelRate;
								paint.IsAntialias = true;
								paint.TextAlign = SKTextAlign.Center;
								paint.Typeface = Font;

								var TextRadius = Radius + paint.TextSize;
								var TextCenter = Center + AngleRadiusToPoint(SatelliteText.Angle + OriginAngle, TextRadius);

								canvas.DrawText
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

				//	パイ・テキストの描画
				if (null == Image)
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
									paint.TextSize = 14.0f * PhysicalPixelRate;
									paint.IsAntialias = true;
									paint.Color = ToSKColor(Color.White);
									paint.TextAlign = SKTextAlign.Center;
									paint.Typeface = Font;

									var CenterAngle = (CurrentAngle + NextAngle) / 2.0f;
									var HalfRadius = Radius / 2.0f;
									var TextCenter = Center + AngleRadiusToPoint(CenterAngle, HalfRadius);

									if (!String.IsNullOrWhiteSpace(Pie.Text))
									{
										canvas.DrawText
										(
											Pie.Text,
											TextCenter.X,
											TextCenter.Y - (paint.TextSize / 2.0f),
											paint
										);
									}
									if (!String.IsNullOrWhiteSpace(Pie.DisplayVolume))
									{
										canvas.DrawText
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

				if (null != Image)
				{
					var ImageSizeBase = Radius / Phi;
					using (var data = new SKData(Image))
					using (var bitmap = SKBitmap.Decode(data))
					using (var paint = new SKPaint())
					{
						var Rect = SKRect.Create
						(
							Center.X - ImageSizeBase,
							Center.Y - ImageSizeBase,
							ImageSizeBase * 2.0f,
							ImageSizeBase * 2.0f
						);
						canvas.DrawBitmap(bitmap, Rect, paint);
					}
				}
			}
		}

		public View AsView()
		{
			return GraphFrame;
		}
	}

	class AlphaCircleGraphView : SKCanvasView
	{
		private AlphaCircleGraph Owner;
		public AlphaCircleGraphView(AlphaCircleGraph aOwner)
		{
			Owner = aOwner;
		}
		protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
		{
			Owner.Draw(e.Surface.Canvas);
		}

	}
}

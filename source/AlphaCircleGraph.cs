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

		byte[] ImageBytes;
		SKData ImageData;
		SKBitmap ImageBitmap;

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

		public byte[] Image
		{
			get
			{
				return ImageBytes;
			}
			set
			{
				if (ImageBytes != value)
				{
					ImageBitmap?.Dispose();
					ImageBitmap = null;
					ImageData?.Dispose();
					ImageData = null;
					ImageBytes = value;
					if (null != ImageBytes)
					{
						ImageData = new SKData(ImageBytes);
						ImageBitmap = SKBitmap.Decode(ImageData);
					}
				}
			}
		}
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
			var ImageRadius = Radius / Phi;

			//	イメージの描画
			if (null != Image && null != ImageData && null != ImageBitmap)
			{
				using (var paint = new SKPaint())
				{
					var Rect = SKRect.Create
					(
						Center.X - ImageRadius,
						Center.Y - ImageRadius,
						ImageRadius * 2.0f,
						ImageRadius * 2.0f
					);
					canvas.DrawBitmap(ImageBitmap, Rect, paint);
				}
			}

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
						if (Pie.Volume < TotalVolume)
						{
							using (var path = new SKPath())
							{
								if (null != Image)
								{
									path.MoveTo(Center + AngleRadiusToPoint(CurrentAngle, ImageRadius));
									path.LineTo(Center + AngleRadiusToPoint(CurrentAngle, Radius));
									path.ArcTo(SKRect.Create(Center.X - Radius, Center.Y - Radius, Radius * 2.0f, Radius * 2.0f), CurrentAngle, CurrentAngleVolume, false);
									//path.LineTo(Center + AngleRadiusToPoint(NextAngle, Radius));
									path.LineTo(Center + AngleRadiusToPoint(NextAngle, ImageRadius));
									path.ArcTo(SKRect.Create(Center.X - ImageRadius, Center.Y - ImageRadius, ImageRadius * 2.0f, ImageRadius * 2.0f), NextAngle, -CurrentAngleVolume, false);
								}
								else
								{
									path.MoveTo(Center);
									path.LineTo(Center + AngleRadiusToPoint(CurrentAngle, Radius));
									path.ArcTo(SKRect.Create(Center.X - Radius, Center.Y - Radius, Radius * 2.0f, Radius * 2.0f), CurrentAngle, CurrentAngleVolume, false);
									path.LineTo(Center + AngleRadiusToPoint(NextAngle, Radius));
									path.LineTo(Center);
								}
								path.Close();
								canvas.DrawPath(path, paint);
							}
						}
						else
						{
							//	TotalVolume <= Pie.Volume な時に上の処理ではパイが描画されないことがある。
							if (null != Image)
							{
								//	一度に描画しようとしても path が繋がらないので半分に分けて描画する
								using (var path = new SKPath())
								{
									path.MoveTo(Center + AngleRadiusToPoint(0.0f, ImageRadius));
									path.LineTo(Center + AngleRadiusToPoint(0.0f, Radius));
									path.ArcTo(SKRect.Create(Center.X - Radius, Center.Y - Radius, Radius * 2.0f, Radius * 2.0f), 0.0f, 180.0f, false);
									//path.LineTo(Center + AngleRadiusToPoint(180.0f, Radius));
									path.LineTo(Center + AngleRadiusToPoint(180.0f, ImageRadius));
									path.ArcTo(SKRect.Create(Center.X - ImageRadius, Center.Y - ImageRadius, ImageRadius * 2.0f, ImageRadius * 2.0f), 180.0f, -180.0f, false);
									path.Close();
									canvas.DrawPath(path, paint);
								}
								using (var path = new SKPath())
								{
									path.MoveTo(Center + AngleRadiusToPoint(180.0f, ImageRadius));
									path.LineTo(Center + AngleRadiusToPoint(180.0f, Radius));
									path.ArcTo(SKRect.Create(Center.X - Radius, Center.Y - Radius, Radius * 2.0f, Radius * 2.0f), 180.0f, 180.0f, false);
									//path.LineTo(Center + AngleRadiusToPoint(180.0f, Radius));
									path.LineTo(Center + AngleRadiusToPoint(360.0f, ImageRadius));
									path.ArcTo(SKRect.Create(Center.X - ImageRadius, Center.Y - ImageRadius, ImageRadius * 2.0f, ImageRadius * 2.0f), 360.0f, -180.0f, false);
									path.Close();
									canvas.DrawPath(path, paint);
								}
							}
							else
							{
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
							if (null != Image)
							{
								path.ArcTo(SKRect.Create(Center.X - ImageRadius, Center.Y - ImageRadius, ImageRadius * 2.0f, ImageRadius * 2.0f), CurrentAngle, CurrentAngleVolume, false);
								path.MoveTo(Center + AngleRadiusToPoint(CurrentAngle, ImageRadius));
							}
							else
							{
								path.MoveTo(Center);
							}
							path.LineTo(Center + AngleRadiusToPoint(CurrentAngle, Radius));
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

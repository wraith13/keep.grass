using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using System.Threading.Tasks;

#if USE_OXYPLOT
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Xamarin.Forms;
#else
using SkiaSharp;
#endif


namespace keep.grass
{
	public interface VoidPie
	{
		string Text { get; }
		double Volume { get; }
		Color Color { get; }
		string DisplayVolume { get; }
	}
	public interface VoidCircleGraph
	{
		IEnumerable<VoidPie> Data { get; set; }
		void SetStartAngle(float NewStartAngle);

		void Build(double Width, double Height);
		void Update();
		View AsView();
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
#if USE_OXYPLOT
	public class AlphaCircleGraph :VoidCircleGraph
	{
		IEnumerable<VoidPie> Pies;

		PieSeries Pie;
		PlotView GraphView;
		Grid GrahpFrame;

		public AlphaCircleGraph()
		{
		}
		public PieSlice[] MakeSlices()
		{
			return (Pies ?? new VoidPie[] { })
			.Select
			(
				p => new PieSlice
				(
					p.Text,
					p.Volume
				)
				{
					Fill = p.Color.ToOxyColor(),
				}
			).ToArray();

		}
		public void Build(double Width, double Height)
		{
			var GraphSize = new[] { Width, Height }.Min() * 0.6;
			Pie = new PieSeries
			{
				TextColor = OxyColors.White,
				StrokeThickness = 1.0,
				StartAngle = 270,
				AngleSpan = 360,
				Slices = MakeSlices(),
			};
			GraphView = new PlotView
			{
				BackgroundColor = Color.White,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				WidthRequest = GraphSize,
				HeightRequest = GraphSize,
				Margin = new Thickness(24.0),
				Model = new OxyPlot.PlotModel
				{
					Series =
					{
						Pie,
					},
				},
			};
			GrahpFrame = new Grid().HorizontalJustificate
			(
				GraphView
			);
			GrahpFrame.BackgroundColor = Color.White;
			GrahpFrame.VerticalOptions = LayoutOptions.FillAndExpand;
		}

		public IEnumerable<VoidPie> Data
		{
			get
			{
				return Pies;
			}

			set
			{
				Pies = value;
				Pie.Slices = MakeSlices();
				Update();
			}
		}

		public void Update()
		{
			GraphView.Model.InvalidatePlot(true);
		}

		public View AsView()
		{
			return GrahpFrame;
		}
	}
#else
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
	public class AlphaCircleGraph :VoidCircleGraph
	{
		float StartAngle = 0.0f;
		IEnumerable<VoidPie> Pies;
		double GraphSize;
		Image Image;
		Grid GrahpFrame;

		public AlphaCircleGraph()
		{
		}
		public void Build(double Width, double Height)
		{
			GraphSize = new[] { Width, Height }.Min() * 0.6;
			Image = new Image()
			{
				Margin = new Thickness(24),
				WidthRequest = GraphSize,
				HeightRequest = GraphSize,
				BackgroundColor = Color.White,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
			};
			GrahpFrame = new Grid().HorizontalJustificate
			(
				Image
			);
			GrahpFrame.BackgroundColor = Color.White;
			GrahpFrame.HorizontalOptions = LayoutOptions.FillAndExpand;
			GrahpFrame.VerticalOptions = LayoutOptions.FillAndExpand;
			Update();
		}

		public IEnumerable<VoidPie> Data
		{
			get
			{
				return Pies;
			}

			set
			{
				Pies = value;
				Update();
			}
		}

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
        public virtual float GetPhysicalPixelRate()
        {
            return 4.0f; // この数字は適当。本来はちゃんとデバイスごとの物理解像度/論理解像度を取得するべき
        }
        public virtual SKColorType GetDeviceColorType()
        {
            return SKColorType.Rgba8888;
        }
		public double GetTotalVolume()
		{
			return Pies.Select(Pie => Pie.Volume).Sum();
		}
		public float GetStartAngle()
		{
			return StartAngle - 90.0f;
		}
		public void SetStartAngle(float NewStartAngle)
		{
			StartAngle = NewStartAngle;
		}
        public void Update()
		{
			if (null != Image)
			{
				var PhysicalPixelRate = GetPhysicalPixelRate();
				var DrawGraphSize = (float)(GraphSize * PhysicalPixelRate);
				var Radius = (DrawGraphSize / 2.0f);// /1.6180339887f; // 1.6180339887f は黄金比 
				var Center = new SKPoint(DrawGraphSize / 2.0f, DrawGraphSize / 2.0f);
				using (var surface = SKSurface.Create((int)DrawGraphSize, (int)DrawGraphSize, GetDeviceColorType(), SKAlphaType.Premul))
				{
					var canvas = surface.Canvas;
					if (null == Pies || !Pies.Any())
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
						var TotalVolume = GetTotalVolume();
						var CurrentAngle = GetStartAngle();
						foreach (var Pie in Pies)
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
									path.AddArc(SKRect.Create(Center.X - Radius, Center.Y - Radius, Radius * 2.0f, Radius * 2.0f), CurrentAngle, CurrentAngleVolume);
									path.MoveTo(Center);
									path.LineTo(Center + AngleRadiusToPoint(CurrentAngle, Radius));
									path.LineTo(Center + AngleRadiusToPoint(NextAngle, Radius));
									path.LineTo(Center);
									path.Close();
									canvas.DrawPath(path, paint);
								}
							}
							CurrentAngle = NextAngle;
						}

						CurrentAngle = GetStartAngle();
						foreach (var Pie in Pies)
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
						using (var FontSource = AlphaFactory.GetApp().GetFontStream())
						{
							using (var FontStream = new SKManagedStream(FontSource))
							{
								using (var Font = SKTypeface.FromStream(FontStream))
								{
									foreach (var Pie in Pies)
									{
										var CurrentAngleVolume = (float)((Pie.Volume / TotalVolume) * 360.0);
										var NextAngle = CurrentAngle + CurrentAngleVolume;
										if (!String.IsNullOrWhiteSpace(Pie.Text) || !String.IsNullOrWhiteSpace(Pie.DisplayVolume))
										{
											using (var paint = new SKPaint())
											{
												paint.IsAntialias = true;
												paint.Color = ToSKColor(Pie.Color);
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
												}
											}
										}
										CurrentAngle = NextAngle;
									}
								}
							}
						}
					}
					var ImageData = surface.Snapshot().Encode();

                    Device.BeginInvokeOnMainThread
                    (
                        () =>
                        {
                            Image.Source = ImageSource.FromStream(() => ImageData.AsStream());
                        }
                    );
				}
			}
		}

		public View AsView()
		{
			return GrahpFrame;
		}
	}
#endif
}

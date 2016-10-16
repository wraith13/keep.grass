using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

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
	public class AlphaCircleGraph :VoidCircleGraph
	{
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
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
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
		public void Update()
		{
			var PhysicalPixelRate = 4.0f; // 数字は適当。本来はちゃんとデバイスの
			var DrawGraphSize = (float)(GraphSize * PhysicalPixelRate);
			var Radius = (DrawGraphSize / 2.0f);// /1.6180339887f; // 1.6180339887f は黄金比 
			var Center = new SKPoint(DrawGraphSize /2.0f, DrawGraphSize /2.0f);
			using (var surface = SKSurface.Create((int)DrawGraphSize, (int)DrawGraphSize, SKColorType.Rgba8888, SKAlphaType.Premul))
			{
				var canvas = surface.Canvas;
				if (null == Pies || !Pies.Any())
				{
					using (var paint = new SKPaint())
					{
						paint.IsAntialias = true;
						paint.Color =  ToSKColor(Color.Gray);
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
					var VolumeTotal = Pies.Select(Pie => Pie.Volume).Sum();
					var StartAngle = -90.0f;
					foreach (var Pie in Pies)
					{
						var CurrentAngle = (float)((Pie.Volume / VolumeTotal) * 360.0);
						var NextAngle = StartAngle + CurrentAngle;
						using (var paint = new SKPaint())
						{
							paint.IsAntialias = true;
							paint.Color = ToSKColor(Pie.Color);
							paint.StrokeCap = SKStrokeCap.Round;
							using (var path = new SKPath())
							{
								path.AddArc(SKRect.Create(Center.X -Radius, Center.Y - Radius, Radius *2.0f, Radius * 2.0f), StartAngle, CurrentAngle);
								path.MoveTo(Center.X, Center.Y);
								path.LineTo(Center.X + (Radius * (float)Math.Cos(DegreeToRadian(StartAngle))), Center.Y + (Radius * (float)Math.Sin(DegreeToRadian(StartAngle))));
								path.LineTo(Center.X + (Radius * (float)Math.Cos(DegreeToRadian(NextAngle))), Center.Y + (Radius * (float)Math.Sin(DegreeToRadian(NextAngle))));
								path.LineTo(Center.X, Center.Y);
								path.Close();
								canvas.DrawPath(path, paint);
							}
						}
						StartAngle = NextAngle;
					}

					StartAngle = -90.0f;
					foreach (var Pie in Pies)
					{
						var CurrentAngle = (float)((Pie.Volume / VolumeTotal) * 360.0);
						var NextAngle = StartAngle + CurrentAngle;
						if (!String.IsNullOrWhiteSpace(Pie.Text) || !String.IsNullOrWhiteSpace(Pie.DisplayVolume))
						{
							using (var paint = new SKPaint())
							{
								paint.IsAntialias = true;
								paint.Color = ToSKColor(Pie.Color);
								paint.StrokeCap = SKStrokeCap.Round;
								using (var path = new SKPath())
								{
									paint.TextSize = 12.0f *PhysicalPixelRate;
									paint.IsAntialias = true;
									paint.Color = ToSKColor(Color.White);
									paint.TextAlign = SKTextAlign.Center;

									var CenterAngle = (StartAngle + NextAngle) / 2.0f;
									var HalfRadius = Radius / 2.0f;

									if (!String.IsNullOrWhiteSpace(Pie.Text))
									{
										canvas.DrawText
										(
											Pie.Text,
											Center.X + (HalfRadius * (float)Math.Cos(DegreeToRadian(CenterAngle))),
											Center.Y + (HalfRadius * (float)Math.Sin(DegreeToRadian(CenterAngle))) - (paint.TextSize / 2.0f),
											paint
										);
									}
									if (!String.IsNullOrWhiteSpace(Pie.DisplayVolume))
									{
										canvas.DrawText
										(
											Pie.DisplayVolume,
											Center.X + (HalfRadius * (float)Math.Cos(DegreeToRadian(CenterAngle))),
											Center.Y + (HalfRadius * (float)Math.Sin(DegreeToRadian(CenterAngle))) + (paint.TextSize / 2.0f),
											paint
										);
									}
								}
							}
						}
						StartAngle = NextAngle;
					}
				}
				var ImageData = surface.Snapshot().Encode();
				Image.Source = ImageSource.FromStream(() => ImageData.AsStream());
			}
		}

		public View AsView()
		{
			return GrahpFrame;
		}
	}
#endif
}

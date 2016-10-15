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
			return String.Format
			(
				"{0:D2}:{1:D2}:{2:D2}",
				Math.Floor(a.TotalHours),
				a.Minutes,
				a.Seconds
			);
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
		Image Image;
		double GraphSize;

		public AlphaCircleGraph()
		{
		}
		public void Build(double Width, double Height)
		{
			GraphSize = new[] { Width, Height }.Min() * 0.6;
			Image = new Image()
			{
				WidthRequest = GraphSize,
				HeightRequest = GraphSize,
				BackgroundColor = Color.White,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};
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

		public void Update()
		{
			using (var surface = SKSurface.Create((int)GraphSize, (int)GraphSize, SKColorType.Rgba8888, SKAlphaType.Premul))
			{
				var canvas = surface.Canvas;
				using (var paint = new SKPaint())
				{
					paint.IsAntialias = true;
					paint.Color = new SKColor(0x2c, 0x3e, 0x50);
					paint.StrokeCap = SKStrokeCap.Round;

					// create the Xamagon path
					using (var path = new SKPath())
					{
						path.MoveTo(71.4311121f, 56f);
						path.CubicTo(68.6763107f, 56.0058575f, 65.9796704f, 57.5737917f, 64.5928855f, 59.965729f);
						path.LineTo(43.0238921f, 97.5342563f);
						path.CubicTo(41.6587026f, 99.9325978f, 41.6587026f, 103.067402f, 43.0238921f, 105.465744f);
						path.LineTo(64.5928855f, 143.034271f);
						path.CubicTo(65.9798162f, 145.426228f, 68.6763107f, 146.994582f, 71.4311121f, 147f);
						path.LineTo(114.568946f, 147f);
						path.CubicTo(117.323748f, 146.994143f, 120.020241f, 145.426228f, 121.407172f, 143.034271f);
						path.LineTo(142.976161f, 105.465744f);
						path.CubicTo(144.34135f, 103.067402f, 144.341209f, 99.9325978f, 142.976161f, 97.5342563f);
						path.LineTo(121.407172f, 59.965729f);
						path.CubicTo(120.020241f, 57.5737917f, 117.323748f, 56.0054182f, 114.568946f, 56f);
						path.LineTo(71.4311121f, 56f);
						path.Close();

						// draw the Xamagon path
						canvas.DrawPath(path, paint);
					}
				}
				var ImageData = surface.Snapshot().Encode();
				Image.Source = ImageSource.FromStream(() => ImageData.AsStream());
			}
		}

		public View AsView()
		{
			return Image;
		}
	}
#endif
}

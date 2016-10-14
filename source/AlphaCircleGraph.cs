using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

#if USE_OXYPLOT
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Xamarin.Forms;
#endif


namespace keep.grass
{
	public class AlphaPie
	{
		public string Text { get; set; }
		public double Volume { get; set; }
		public Color Color { get; set; }
		public string DisplayVolume { get; set; }
	}
	public interface VoidCircleGraph
	{
		IEnumerable<AlphaPie> Data { get; set; }
		void Build(double Width, double Height);
		void Update();
		View AsView();
	}
	public class AlphaCircleGraph :View, VoidCircleGraph
	{
		IEnumerable<AlphaPie> Pies;

		PieSeries Pie;
		PlotView GraphView;
		Grid GrahpFrame;

		public AlphaCircleGraph()
		{
		}
		public PieSlice[] MakeSlices()
		{
			return (Pies ?? new AlphaPie[] { })
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

		public IEnumerable<AlphaPie> Data
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
}

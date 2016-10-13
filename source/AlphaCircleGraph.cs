using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace keep.grass
{
	public class AlphaPie
	{
		string Text { get; set; }
		double Volume { get; set; }
		string DisplayVolume { get; set; }
	}
	public interface VoidCircleGraph
	{
		IEnumerable<AlphaPie> Data { get; set; }
		View AsView();
	}
	public class AlphaCircleGraph
	{
		public AlphaCircleGraph()
		{
		}
	}
}

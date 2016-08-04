using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace keep.grass
{
	public class ResponsiveLayout : StackLayout
	{
		public int MaxColumnWidth;
		public int MinColumnWidth;

		public List<Layout> BlockList = new List<Layout>();
		public List<StackLayout> ColumnStackList = new List<StackLayout>();

		public ResponsiveLayout()
		{
			Orientation = StackOrientation.Horizontal;
		}
	}
}


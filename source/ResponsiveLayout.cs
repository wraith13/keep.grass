using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace keep.grass
{
	public class ResponsiveLayout : StackLayout
	{
		public double MaxColumnWidth;
		public double MinColumnWidth;

		public List<Layout> BlockList = new List<Layout>();
		public List<StackLayout> ColumnStackList = new List<StackLayout>();

		public ResponsiveLayout()
		{
			Orientation = StackOrientation.Horizontal;
		}

		public void Response()
		{
			var ColumnSize =
				MaxColumnWidth <= Width ?
					1:
					Math.Min((int)(Width /MinColumnWidth), BlockList.Count);
			
			ColumnStackList.Clear();
			ColumnStackList.Add
			(
				new StackLayout
				{
					Children =
					{
						BlockList.FirstOrDefault(),
					}
				}
			);
		}
	}
}


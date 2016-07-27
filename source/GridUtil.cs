using System;
using System.Linq;

using Xamarin.Forms;

namespace keep.grass
{
	public static class GridUtil
	{
		public static Grid SetSingleChild(this Grid grid, View value)
		{
			grid.Children.Add(value);
			return grid;
		}
		public static View GetSingleChild(this Grid grid)
		{
			return grid.Children.SingleOrDefault();
		}

		public static Grid HorizontalJustificate(this Grid grid, params View[] views)
		{
			grid.HorizontalOptions = LayoutOptions.FillAndExpand;
			grid.VerticalOptions = LayoutOptions.Center;
			grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
			for (var i = 0; i < views.Length; ++i)
			{
				grid.ColumnDefinitions.Add
				(
					new ColumnDefinition
					{
						Width = new GridLength(1, GridUnitType.Star),
					}
				);
				grid.Children.Add(views[i], i, 0);
			}

			return grid;
		}
	}
}


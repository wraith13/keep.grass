using System;
using System.Linq;

using Xamarin.Forms;

namespace keep.grass
{
	public static class GridEx
	{
		public enum Justificate
		{
			Even,
			Odd,
		}

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
		public static Grid OddHorizontalJustificate(this Grid grid, params View[] views)
		{
			grid.HorizontalOptions = LayoutOptions.FillAndExpand;
			grid.VerticalOptions = LayoutOptions.Center;
			grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
			grid.ColumnDefinitions.Add
			(
				new ColumnDefinition
				{
					Width = new GridLength(1, GridUnitType.Star),
				}
			);
			for (var i = 0; i < views.Length; ++i)
			{
				grid.ColumnDefinitions.Add
				(
					new ColumnDefinition
					{
						Width = new GridLength(2, GridUnitType.Star),
					}
				);
				grid.Children.Add(views[i], i +1, 0);
			}
			grid.ColumnDefinitions.Add
			(
				new ColumnDefinition
				{
					Width = new GridLength(1, GridUnitType.Star),
				}
			);
			return grid;
		}
		public static Grid HorizontalJustificate(this Grid grid, Justificate Option, params View[] views)
		{
			switch(Option)
			{
				case Justificate.Even:
					grid.HorizontalJustificate(views);
					break;
				case Justificate.Odd:
					grid.OddHorizontalJustificate(views);
					break;
			}
			return grid;
		}
		public static Grid VerticalJustificate(this Grid grid, params View[] views)
		{
			grid.HorizontalOptions = LayoutOptions.Center;
			grid.VerticalOptions = LayoutOptions.FillAndExpand;
			grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
			for (var i = 0; i < views.Length; ++i)
			{
				grid.RowDefinitions.Add
				(
					new RowDefinition
					{
						Height = new GridLength(1, GridUnitType.Star),
					}
				);
				grid.Children.Add(views[i], 0, i);
			}
			return grid;
		}
		public static Grid OddVerticalJustificate(this Grid grid, params View[] views)
		{
			grid.HorizontalOptions = LayoutOptions.Center;
			grid.VerticalOptions = LayoutOptions.FillAndExpand;
			grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
			grid.RowDefinitions.Add
			(
				new RowDefinition
				{
					Height = new GridLength(1, GridUnitType.Star),
				}
			);
			for (var i = 0; i < views.Length; ++i)
			{
				grid.RowDefinitions.Add
				(
					new RowDefinition
					{
						Height = new GridLength(2, GridUnitType.Star),
					}
				);
				grid.Children.Add(views[i], 0, i +1);
			}
			grid.RowDefinitions.Add
			(
				new RowDefinition
				{
					Height = new GridLength(1, GridUnitType.Star),
				}
			);
			return grid;
		}
		public static Grid VerticalJustificate(this Grid grid, Justificate Option, params View[] views)
		{
			switch (Option)
			{
				case Justificate.Even:
					grid.VerticalJustificate(views);
					break;
				case Justificate.Odd:
					grid.OddVerticalJustificate(views);
					break;
			}
			return grid;
		}
		public static Grid LineJustificate(this Grid grid, StackOrientation Orientation, Justificate Option, params View[] views)
		{
			switch (Orientation)
			{
				case StackOrientation.Horizontal:
					grid.HorizontalJustificate(Option, views);
					break;
				case StackOrientation.Vertical:
					grid.VerticalJustificate(Option, views);
					break;
			}
			return grid;
		}
	}
}


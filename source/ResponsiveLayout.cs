using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace keep.grass
{
	public class ResponsiveCore
	{
		public double MaxColumnWidth;
		public double MinColumnWidth;

		public List<Layout> BlockList = new List<Layout>();

		public ResponsiveCore()
		{
			Orientation = StackOrientation.Horizontal;
			HorizontalOptions = LayoutOptions.Center;
			VerticalOptions = LayoutOptions.CenterAndExpand;
		}

		public void Response(Element element)
		{
			var MaxColumnSize =
				MaxColumnWidth <= element.Width ?
					1:
					Math.Min((int)(Width /MinColumnWidth), BlockList.Count);
			
			var ColumnSize = 0;
			do
			{
				++ColumnSize;
				Children.Clear();
				for (var i = 0; i < ColumnSize; ++i)
				{
					var CurrentStack = new StackLayout();
					foreach (var Block in BlockList.Where((v, index) => i == index % ColumnSize))
					{
						CurrentStack.Children.Add(Block);
					}
					Children.Add(CurrentStack);
				}
			}
			while
			(
				Height < Children.Select(i => i.Height).Sum() &&
				ColumnSize < MaxColumnSize
			);

			var ColumnWidth = Math.Min(Width / ColumnSize, MaxColumnWidth);
			foreach(var i in Children)
			{
				i.MinimumWidthRequest = MinColumnWidth;
				i.WidthRequest = ColumnWidth;
			}
		}
	}
	public class ResponsiveLayout : StackLayout
	{
	}
	public class ResponsiveTableView : TableView
	{
		private List<TableSection> ChildrenValue = new List<TableSection>();
		public IList<TableSection> Children
		{
			get
			{
				return ChildrenValue;
			}
		}

		public ResponsiveTableView
		{
		}
	}
	public class ResponsiveCell : ViewCell
	{
	}
}


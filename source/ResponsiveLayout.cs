using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace keep.grass
{
	public abstract class ResponsiveEelement
	{
		public abstract double Width { get; }
		public abstract double Height { get; }

		public abstract double MinimumWidthRequest { set; }
		public abstract double WidthRequest { set; }

		public virtual void Response() { }
	}
	public class ResponsiveBlock
	{
		public double Width
		{
			get
			{
				return Elements.Select(i => i.Width).Max();
			}
		}
		public double Height
		{
			get
			{
				return Elements.Select(i => i.Height).Sum();
			}
		}

		public double MinimumWidthRequest
		{
			set
			{
				foreach(var i in Elements)
				{
					i.MinimumWidthRequest = value;
				}
			}
		}
		public double WidthRequest
		{
			set
			{
				foreach (var i in Elements)
				{
					i.WidthRequest = value;
				}
			}
		}

		public List<ResponsiveEelement> Elements = new List<ResponsiveEelement>();

		public ResponsiveBlock(IEnumerable<ResponsiveEelement> aElements)
		{
			Elements.AddRange(aElements);
		}
		public void Response()
		{
			foreach (var i in Elements)
			{
				i.Response();
			}
		}
	}
	public abstract class ResponsiveContainer
	{
		public double MaxColumnWidth;
		public double MinColumnWidth;

		public List<ResponsiveEelement> Elements = new List<ResponsiveEelement>();
		public List<ResponsiveBlock> Blocks = new List<ResponsiveBlock>();

		public ResponsiveContainer()
		{
			//Orientation = StackOrientation.Horizontal;
			//HorizontalOptions = LayoutOptions.Center;
			//VerticalOptions = LayoutOptions.CenterAndExpand;
		}

		public virtual ResponsiveBlock MakeBlock(IEnumerable<ResponsiveEelement> BlockElements)
		{
			return new ResponsiveBlock(BlockElements);
		}
		public void Response(double Width, double Height)
		{
			var MaxColumnSize =
				MaxColumnWidth <= Width ?
					1:
					Math.Min((int)(Width /MinColumnWidth), Elements.Count);
			
			var ColumnSize = 0;
			do
			{
				++ColumnSize;
				Blocks.Clear();
				for (var i = 0; i < ColumnSize; ++i)
				{
					Blocks.Add
			      	(
			      		MakeBlock
				      	(
				      		Elements.Where((v, index) => i == index % ColumnSize)
				     	)
			     	);
				}
			}
			while
			(
				Height < Blocks.Select(i => i.Height).Sum() &&
				ColumnSize < MaxColumnSize
			);

			var ColumnWidth = Math.Min(Width / ColumnSize, MaxColumnWidth);
			foreach(var i in Blocks)
			{
				i.MinimumWidthRequest = MinColumnWidth;
				i.WidthRequest = ColumnWidth;
				i.Response();
			}
		}
	}
	public class ResponsiveViewElement<viewT> :ResponsiveEelement where viewT :View
	{
		viewT view;
		public ResponsiveViewElement(viewT a_view)
		{
			view = a_view;
		}

		public override double Width
		{
			get
			{
				return view.Width;
			}
		}
		public override double Height
		{
			get
			{
				return view.Height;
			}
		}

		public override double MinimumWidthRequest
		{
			set
			{
				view.MinimumWidthRequest = value;
			}
		}
		public override double WidthRequest
		{
			set
			{
				view.WidthRequest = value;
			}
		}

		public viewT View
		{
			get
			{
				return view;
			}
		}
	}
	public class ResponsiveLayout : StackLayout
	{
	}
	public class ResponsiveTableView : ResponsiveViewElement<TableView>
	{
		private List<TableSection> ChildrenValue = new List<TableSection>();
		public IList<TableSection> Children
		{
			get
			{
				return ChildrenValue;
			}
		}

		public ResponsiveTableView(TableView a_view) :base(a_view)
		{
		}
	}
	public class ResponsiveCell : ViewCell
	{
	}
}


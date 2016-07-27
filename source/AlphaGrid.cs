using System;
using System.Linq;

using Xamarin.Forms;

namespace keep.grass
{
	public class AlphaGrid :Grid
	{
		public AlphaGrid()
		{
		}

		public View SingleChild
		{
			set
			{
				Children.Add(value);
			}
			get
			{
				return Children.SingleOrDefault();
			}
		}

		public AlphaGrid HorizontalJustificate(params View[] views)
		{
			HorizontalOptions = LayoutOptions.FillAndExpand;
			VerticalOptions = LayoutOptions.Center;
			RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
			for (var i = 0; i < views.Length; ++i)
			{
				ColumnDefinitions.Add
				(
					new ColumnDefinition
					{
						Width = new GridLength(1, GridUnitType.Star),
					}
				);
				Children.Add(views[i], i, 0);
			}

			return this;
		}
	}
}


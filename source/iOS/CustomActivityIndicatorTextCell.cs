using System;
using Xamarin.Forms;

namespace keep.grass.iOS
{
	public class CustomActivityIndicatorTextCell : AlphaActivityIndicatorTextCell
	{
		public CustomActivityIndicatorTextCell()
		{
			Layout.Padding = new Thickness(20, 12, 0, 12);
		}
	}
}


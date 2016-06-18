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
	}
}


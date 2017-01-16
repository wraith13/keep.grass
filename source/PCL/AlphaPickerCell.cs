using System;

using Xamarin.Forms;

namespace keep.grass
{
	public class AlphaPickerCell : ViewCell
	{
		public Picker Picker = new Picker();

		public AlphaPickerCell() : base()
		{
            View = new Grid().SetSingleChild
			(
				new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					VerticalOptions = LayoutOptions.Center,
					Padding = new Thickness(20, 0, 0, 0),
					Children =
					{
						Picker,
					},
				}
			);
            Picker.WidthRequest = 240;
        }

        protected override void OnTapped()
		{
			base.OnTapped();
			Picker.Focus();
		}

		public String Title
		{
			get
			{
				return Picker.Title;
			}
			set
			{
				Picker.Title = value;
			}
		}
		public System.Collections.Generic.IList<string> Items
		{
			get
			{
				return Picker.Items;
			}
		}

		public int SelectedIndex
		{
			get
			{
				return Picker.SelectedIndex;
			}
			set
			{
				Picker.SelectedIndex = value;
			}
		}
	}
}


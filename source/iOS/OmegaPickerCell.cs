using System;
using Xamarin.Forms;

namespace keep.grass.iOS
{
	public class OmegaPickerCell : AlphaPickerCell
	{
		Label TextLabel = new Label();

		public OmegaPickerCell() : base()
		{
			View = new AlphaGrid
			{
				SingleChild = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					VerticalOptions = LayoutOptions.Center,
					Padding = new Thickness(20, 0, 0, 0),
					Children =
					{
						TextLabel,
						Picker,
					},
				}
			};

			TextLabel.VerticalOptions = LayoutOptions.Center;
			Picker.IsVisible = false;
			Picker.SelectedIndexChanged += (sender, e) => TextLabel.Text = Picker.Items[Picker.SelectedIndex];
		}
	}
}


using System;

using Xamarin.Forms;

namespace keep.grass
{
	public class AlphaPickerCell : ViewCell
	{
		Label TextLabel = new Label();
		public Picker Picker = new Picker();

		public AlphaPickerCell() : base()
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
	}
}


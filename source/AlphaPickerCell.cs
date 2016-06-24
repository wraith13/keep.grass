using System;

using Xamarin.Forms;

namespace keep.grass
{
	public class AlphaPickerCell : ViewCell
	{
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
						Picker,
					},
				}
			};
		}

		private Command CommandValue = null;
		public Command Command
		{
			set
			{
				CommandValue = value;
			}
			get
			{
				return CommandValue;
			}
		}
		protected override void OnTapped()
		{
			base.OnTapped();
			if (null != CommandValue)
			{
				CommandValue.Execute(this);
			}
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


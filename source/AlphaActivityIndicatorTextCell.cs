using System;

using Xamarin.Forms;

namespace keep.grass
{
	public class AlphaActivityIndicatorTextCell :ViewCell
	{
		protected ActivityIndicator Indicator = new ActivityIndicator();
		protected Label TextLabel = new Label();

		public AlphaActivityIndicatorTextCell() :base()
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
						Indicator,
						TextLabel,
					},
				}
			};
            Indicator.VerticalOptions = LayoutOptions.Center;
			Indicator.HorizontalOptions = LayoutOptions.Center;
			TextLabel.VerticalOptions = LayoutOptions.Center;
		}

        public void ShowText()
		{
			Indicator.IsRunning = false;
			Indicator.IsVisible = false;
			TextLabel.IsVisible = true;
		}
		public void ShowIndicator()
		{
			Indicator.IsRunning = true;
			Indicator.IsVisible = true;
			TextLabel.IsVisible = false;
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

		public String Text
		{
			get
			{
				return TextLabel.Text;
			}
			set
			{
				TextLabel.Text = value;
			}
		}
		public Color TextColor
		{
			get
			{
				return TextLabel.TextColor;
			}
			set
			{
				TextLabel.TextColor = value;
			}
		}
	}
}


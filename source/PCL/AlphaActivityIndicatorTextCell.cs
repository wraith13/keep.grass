using System;

using Xamarin.Forms;

namespace keep.grass
{
	public class AlphaActivityIndicatorTextCell :ViewCell
	{
		protected ActivityIndicator Indicator = new ActivityIndicator();
		protected Label TextLabel = new Label();
		protected Image RefreshImage = new Image();

		public AlphaActivityIndicatorTextCell() :base()
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
						Indicator,
						TextLabel,
						RefreshImage,
					},
				}
			);
            Indicator.VerticalOptions = LayoutOptions.Center;
			Indicator.HorizontalOptions = LayoutOptions.Center;
			TextLabel.VerticalOptions = LayoutOptions.Center;
			TextLabel.HorizontalOptions = LayoutOptions.StartAndExpand;
			RefreshImage.VerticalOptions = LayoutOptions.Center;
			RefreshImage.HorizontalOptions = LayoutOptions.End;
			RefreshImage.Source = AlphaFactory.GetApp().GetRefreshImageSource();
			RefreshImage.IsVisible = null != CommandValue;

            Indicator.IsVisible = false;
        }

        public void ShowText()
		{
			Indicator.IsRunning = false;
			Indicator.IsVisible = false;
			TextLabel.IsVisible = true;
			RefreshImage.IsVisible = null != CommandValue;
		}
		public void ShowIndicator()
		{
			Indicator.IsRunning = true;
			Indicator.IsVisible = true;
			TextLabel.IsVisible = false;
			RefreshImage.IsVisible = false;
		}

		private Command CommandValue = null;
		public Command Command
		{
			set
			{
				CommandValue = value;
				RefreshImage.IsVisible = null != CommandValue;
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


using System;

using Xamarin.Forms;
using ImageCircle.Forms.Plugin.Abstractions;

namespace keep.grass
{
	public class AlphaCircleImageCell : ViewCell
	{
		CircleImage Image = new CircleImage();
		Label TextLabel = new Label();

		public AlphaCircleImageCell() : base()
		{
			View = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				VerticalOptions = LayoutOptions.Center,
				Padding = new Thickness(20, 2, 0, 2),
				Children =
				{
					Image,
					TextLabel,
				},
			};

			//この指定はUWP版としての継承先へ移動すること
            //Image.HeightRequest = 48;
            //Image.WidthRequest = 48;

            Image.VerticalOptions = LayoutOptions.Center;
			TextLabel.VerticalOptions = LayoutOptions.Center;
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

		public ImageSource ImageSource
		{
			get
			{
				return Image.Source;
			}
			set
			{
				Image.Source = value;
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


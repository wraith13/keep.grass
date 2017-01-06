using System;
using System.Reflection;
using System.Linq;

using Xamarin.Forms;
using ImageCircle.Forms.Plugin.Abstractions;

namespace keep.grass
{
    public class AlphaCircleImageCell : ViewCell
	{
		public static readonly BindableProperty ImageSourceProperty = typeof(AlphaCircleImageCell).GetRuntimeProperty("ImageSource").CreateBindableProperty();
		public static readonly BindableProperty TextProperty = typeof(AlphaCircleImageCell).GetRuntimeProperty("Text").CreateBindableProperty();

		protected Image Image = AlphaFactory.MakeCircleImage();
		protected Label TextLabel = new Label();
		protected Image OptionImage = new Image();

		public AlphaCircleImageCell() : base()
		{
			View = new Grid().SetSingleChild
			(
				new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					VerticalOptions = LayoutOptions.Center,
					Padding = new Thickness(20, 2, 0, 2),
					Children =
					{
						Image,
						TextLabel,
						OptionImage,
					},
				}
			);

            Image.IsVisible = null != Image.Source;
            Image.VerticalOptions = LayoutOptions.Center;
			TextLabel.VerticalOptions = LayoutOptions.Center;
			TextLabel.HorizontalOptions = LayoutOptions.StartAndExpand;
			OptionImage.VerticalOptions = LayoutOptions.Center;
			OptionImage.HorizontalOptions = LayoutOptions.End;
			OptionImage.Source = AlphaFactory.GetApp().GetRightImageSource();
			OptionImage.IsVisible = null != CommandValue;
		}

		private Command CommandValue = null;
		public Command Command
		{
			set
			{
				CommandValue = value;
				OptionImage.IsVisible = null != CommandValue && null != OptionImage.Source;
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
                Image.IsVisible = null != Image.Source;
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
		public ImageSource OptionImageSource
		{
			get
			{
				return OptionImage.Source;
			}
			set
			{
				OptionImage.Source = value;
				OptionImage.IsVisible = null != CommandValue && null != OptionImage.Source;
			}
		}
	}
}


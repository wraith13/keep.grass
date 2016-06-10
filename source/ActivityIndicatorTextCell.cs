using System;

using Xamarin.Forms;

namespace keep.grass
{
	public class ActivityIndicatorTextCell :ViewCell
	{
		ActivityIndicator Indicator = new ActivityIndicator();
		Label TextLabel = new Label();

		public ActivityIndicatorTextCell() :base()
		{
			View = new StackLayout
			{
				Children =
				{
					Indicator,
					TextLabel,
				}
			};
		}

		public Command Command
		{
			set;
			get;
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


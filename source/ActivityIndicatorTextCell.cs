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


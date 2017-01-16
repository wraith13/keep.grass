using System;
using Xamarin.Forms;

namespace keep.grass
{
	public class AlphaActivityIndicatorButton :StackLayout
	{
		protected ActivityIndicator Indicator = new ActivityIndicator();
		protected Button Button = new Button();

		public AlphaActivityIndicatorButton() :base()
		{
			Orientation = StackOrientation.Horizontal;
			VerticalOptions = LayoutOptions.Center;
			HorizontalOptions = LayoutOptions.FillAndExpand;
			Padding = new Thickness(20, 0, 0, 0);
			Children.Add(Indicator);
			Children.Add(Button);

			Indicator.VerticalOptions = LayoutOptions.Center;
			Indicator.HorizontalOptions = LayoutOptions.FillAndExpand;
			Button.VerticalOptions = LayoutOptions.Center;
			Button.HorizontalOptions = LayoutOptions.FillAndExpand;

			Indicator.IsVisible = false;
		}

		public void ShowText()
		{
			Indicator.IsRunning = false;
			Indicator.IsVisible = false;
			Button.IsVisible = true;
		}
		public void ShowIndicator()
		{
			Indicator.IsRunning = true;
			Indicator.IsVisible = true;
			Button.IsVisible = false;
		}

		public System.Windows.Input.ICommand Command
		{
			set
			{
				Button.Command = value;
			}
			get
			{
				return Button.Command;
			}
		}

		public String Text
		{
			get
			{
				return Button.Text;
			}
			set
			{
				Button.Text = value;
			}
		}
		public Color TextColor
		{
			get
			{
				return Button.TextColor;
			}
			set
			{
				Button.TextColor = value;
			}
		}
	}
}

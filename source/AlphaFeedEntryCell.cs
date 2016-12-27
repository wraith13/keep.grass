using System;
using System.Linq;
using Xamarin.Forms;

namespace keep.grass
{
	public class VoidFeedEntryCell : ViewCell
	{
		public virtual GitHub.Feed.Entry Entry { get; set; }
	}
	public class AlphaFeedEntryCell : VoidFeedEntryCell
	{
		protected Image Image = AlphaFactory.MakeCircleImage();
		protected Label TextLabel = new Label();
		protected Image OptionImage = new Image();

		public override GitHub.Feed.Entry Entry
		{
			set
			{
				base.Entry = value;
				Text = Entry.Title;
				Command = new Command(o => Device.OpenUri(new Uri(Entry?.LinkList.Select(i => i.Href).FirstOrDefault())));
			}
		}

		public AlphaFeedEntryCell() : base()
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
			OptionImage.Source = AlphaFactory.GetApp().GetExportImageSource();
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


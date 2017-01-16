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
		AlphaDomain Domain = AlphaFactory.MakeSureDomain();
		AlphaApp Root = AlphaFactory.MakeSureApp();

		protected Image Image = AlphaFactory.MakeCircleImage();
		protected Label UpdatedLabel = new Label();
		protected Label TitleLabel = new Label();
		protected StackLayout DetailStack = new StackLayout
		{
			Orientation = StackOrientation.Vertical,
			VerticalOptions = LayoutOptions.Start,
			HorizontalOptions = LayoutOptions.StartAndExpand,
		};
		protected Image OptionImage = new Image();

		public override GitHub.Feed.Entry Entry
		{
			set
			{
				base.Entry = value;
				ImageSource = Root.GetOcticonImageSource(Entry.Content.OctIcon);
				TitleLabel.Text = Entry.Title;
				UpdatedLabel.Text = Domain.ToString(Entry.Updated);
				UpdatedLabel.TextColor = Entry.IsContribution ?
					AlphaDomain.MakeLeftTimeColor(Entry.Updated.AddDays(1) - DateTime.Now) :
					Color.Gray;
				DetailStack.Children.Clear();
				foreach(var i in Entry.Content.Details)
				{
					DetailStack.Children.Add
					(
						new Label
						{
							Text = "・" +i,
							VerticalOptions = LayoutOptions.Start,
							HorizontalOptions = LayoutOptions.Start,
							LineBreakMode = LineBreakMode.TailTruncation,
							FontSize = 13,
						}
					);
				};
				Command = new Command(o => Device.OpenUri(new Uri(Entry?.LinkList.Select(i => i.Href).FirstOrDefault())));
			}
		}

		public AlphaFeedEntryCell() : base()
		{
			Image.IsVisible = null != Image.Source;
			Image.VerticalOptions = LayoutOptions.Center;
			Image.HeightRequest = 40;
			Image.WidthRequest = 40;
			TitleLabel.VerticalOptions = LayoutOptions.Start;
			TitleLabel.HorizontalOptions = LayoutOptions.Start;
			TitleLabel.LineBreakMode = LineBreakMode.TailTruncation;
			TitleLabel.FontSize = 16;
			UpdatedLabel.VerticalOptions = LayoutOptions.Start;
			UpdatedLabel.HorizontalOptions = LayoutOptions.StartAndExpand;
			UpdatedLabel.FontSize = 13;
			OptionImage.VerticalOptions = LayoutOptions.Center;
			OptionImage.HorizontalOptions = LayoutOptions.End;
			OptionImage.Source = AlphaFactory.GetApp().GetExportImageSource();
			OptionImage.IsVisible = null != CommandValue;
			OptionImage.HeightRequest = 40;
			OptionImage.WidthRequest = 40;

			View = new Grid().SetSingleChild
			(
				new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					VerticalOptions = LayoutOptions.Center,
					Padding = new Thickness(10, 4, 0, 4),
					Children =
					{
						Image,
						new StackLayout
						{
							Orientation = StackOrientation.Vertical,
							VerticalOptions = LayoutOptions.Start,
							HorizontalOptions = LayoutOptions.StartAndExpand,
							Children =
							{
								TitleLabel,
								DetailStack,
								UpdatedLabel,
							}
						},
						OptionImage,
					},
				}
			);
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


using System;
using System.Linq;
using System.Collections.Generic;
using Xamarin.Forms;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using RuyiJinguBang;

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

        protected AlphaImageView Image = AlphaFactory.MakeCircleImage();
        protected Label UpdatedLabel = new Label();
        protected Label TitleLabel = new Label();
        protected StackLayout DetailStack = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.StartAndExpand,
        };
        protected AlphaImageView OptionImage = new AlphaImageView();

        public override GitHub.Feed.Entry Entry
        {
            set
            {
                base.Entry = value;
                ImageSource = Root.GetOcticonImageSource(Entry.Content.OctIcon);
                TitleLabel.Text = Entry.Title;
                UpdatedLabel.Text = Domain.ToString(Entry.Updated);
                DetailStack.Children.Clear();
                foreach (var i in Entry.Content.Details)
                {
                    DetailStack.Children.Add
                    (
                        new Label
                        {
                            Text = "・" + i,
                            VerticalOptions = LayoutOptions.Start,
                            HorizontalOptions = LayoutOptions.Start,
                            LineBreakMode = LineBreakMode.TailTruncation,
                            FontSize = 13,
                        }
                    );
                };
                Command = new Command
                (
                    o =>
                    {
                        Analytics.TrackEvent(
                            name: "[Clicked] Activity",
                            properties: new Dictionary<string, string> { { "Category", "ColumnClick" }, { "Screen", "FeedPage" } }
                        );
                        Xamarin.Forms.Device.OpenUri(new Uri(Entry?.LinkList.Select(i => i.Href).FirstOrDefault()));
                    }
                );
                AlphaTheme.Apply(this);
                UpdatedLabel.TextColor = Entry.IsContribution ?
                    AlphaDomain.MakeLeftTimeColor(Entry.Updated.AddDays(1) - DateTime.Now) :
                    Color.Gray;
            }
        }

        public AlphaFeedEntryCell() : base()
        {
            Image.IsVisible = null != Image.ImageBytes;
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
            OptionImage.ImageSource = AlphaFactory.GetApp().GetExportImageSource();
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
                OptionImage.IsVisible = null != CommandValue && null != OptionImage.ImageBytes;
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
                return Image.ImageSource;
            }
            set
            {
                Image.ImageSource = value;
                Image.IsVisible = null != Image.ImageSource;
            }
        }

        public ImageSource OptionImageSource
        {
            get
            {
                return OptionImage.ImageSource;
            }
            set
            {
                OptionImage.ImageSource = value;
                OptionImage.IsVisible = null != CommandValue && null != OptionImage.ImageSource;
            }
        }
    }
}


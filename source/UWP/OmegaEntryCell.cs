using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using RuyiJinguBang;
using keep.grass.App;

namespace keep.grass.UWP
{
    class OmegaEntryCell : ViewCell, VoidEntryCell
    {
        protected Label TextLabel = new Label();
        protected Entry TextEntry = new Entry();

        public string Label
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
        public string Text
        {
            get
            {
                return TextEntry.Text;
            }
            set
            {
                TextEntry.Text = value;
            }
        }
        public Cell AsCell()
        {
            return this;
        }

        public OmegaEntryCell() : base()
        {
            View = new Grid().SetSingleChild
            (
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.Center,
                    Padding = new Thickness(20, 0, 20, 0),
                    Children =
                    {
                        TextLabel,
                        TextEntry,
                    },
                }
            );

            TextLabel.HorizontalOptions = LayoutOptions.Start;
            TextLabel.VerticalOptions = LayoutOptions.Center;
            TextEntry.HorizontalOptions = LayoutOptions.EndAndExpand;
            TextEntry.VerticalOptions = LayoutOptions.Center;
            TextEntry.WidthRequest = 240;
        }
    }
}

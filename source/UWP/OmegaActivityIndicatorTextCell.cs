using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace keep.grass.UWP
{
    class OmegaActivityIndicatorTextCell : AlphaActivityIndicatorTextCell
    {
        public OmegaActivityIndicatorTextCell()
        {
            View = new Grid().SetSingleChild
            (
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.StartAndExpand,
                            Padding = new Thickness(20, 8, 20, 8),
                            Children =
                            {
                                Indicator,
                                TextLabel,
                            },
                        },
                        RefreshImage,
                    },
                }
            );
            View.SizeChanged += (sender, args) => AdjustIndicatorSize();
            Indicator.HeightRequest = 20;
            RefreshImage.HeightRequest = 48;
            RefreshImage.WidthRequest = 48;
            AdjustIndicatorSize();
        }

        private void AdjustIndicatorSize()
        {
            Indicator.WidthRequest = View.Width - 40;
        }
    }
}

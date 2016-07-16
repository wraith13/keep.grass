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
            ((StackLayout)((AlphaGrid)View).Children.First()).Padding = new Thickness(20, 8, 20, 8);
            View.SizeChanged += (sender, args) => AdjustIndicatorSize();
            Indicator.HeightRequest = 20;
            AdjustIndicatorSize();
        }

        private void AdjustIndicatorSize()
        {
            Indicator.WidthRequest = View.Width - 40;
        }
    }
}

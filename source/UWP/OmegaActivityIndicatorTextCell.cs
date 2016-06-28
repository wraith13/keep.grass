using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace keep.grass.UWP
{
    class OmegaActivityIndicatorTextCell : AlphaActivityIndicatorTextCell
    {
        public OmegaActivityIndicatorTextCell()
        {
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using keep.grass.App;

namespace keep.grass.UWP
{
    class OmegaCircleImageCell : AlphaCircleImageCell
    {
        public OmegaCircleImageCell()
        {
            View.HeightRequest = 48;
            Image.HeightRequest = 48;
            Image.WidthRequest = 48;
        }
    }
}

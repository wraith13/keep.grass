using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using keep.grass.Helpers;
using Xamarin.Forms;
using System.Reflection;
using System.Diagnostics;

namespace keep.grass.UWP
{
    public class OmegaApp : AlphaApp
    {
        public OmegaApp()
        {
        }

        public override Uri GetApplicationStoreUri()
        {
            return new Uri("https://www.microsoft.com/store/apps/9nblggh51p1m");
        }
    }
}

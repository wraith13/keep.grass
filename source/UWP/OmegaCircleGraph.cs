﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace keep.grass.UWP
{
    class OmegaCircleGraph : AlphaCircleGraph
    {
        public override SKColorType GetDeviceColorType()
        {
            return SKColorType.Bgra8888;
        }
    }
}

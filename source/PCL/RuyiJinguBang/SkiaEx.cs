using System;
using System.IO;
using SkiaSharp;
using Xamarin.Forms;

namespace RuyiJinguBang
{
    public static class SkiaEx
    {
        public static void MoveTo(this SKPath Path, SKPoint Point)
        {
            Path.MoveTo(Point.X, Point.Y);
        }
        public static void LineTo(this SKPath Path, SKPoint Point)
        {
            Path.LineTo(Point.X, Point.Y);
        }
    }
}

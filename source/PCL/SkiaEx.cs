using System;
using System.IO;
using SkiaSharp;
using Xamarin.Forms;

namespace keep.grass
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
		public static SKColor ToSKColor(this Color c)
		{
			return new SKColor
			(
				(byte)(c.R * 255),
				(byte)(c.G * 255),
				(byte)(c.B * 255),
				(byte)(c.A * 255)
			);
		}
	}
}

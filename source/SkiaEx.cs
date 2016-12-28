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
		static public ImageSource ImageFromSvg(int width, int height, byte[] source)
		{
			using (var Surface = SKSurface.Create(width, height, SKColorType.Rgba8888, SKAlphaType.Opaque))
			{
				using (var Data = new SKData(source))
				{
					using (var Bitmap = SKBitmap.Decode(Data))
					{
						Surface.Canvas.DrawBitmap(Bitmap, 0, 0);
						using (var SnapShot = Surface.Snapshot())
						{
							using (var ResultData = SnapShot.Encode())
							{
								var Stream = new MemoryStream(ResultData.ToArray());
								return ImageSource.FromStream(() => Stream);
							}
						}
					}
				}
			}
		}
	}
}

using System;
using System.Linq;
using System.Diagnostics;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using RuyiJinguBang;

namespace keep.grass
{
    public abstract class VoidImage : SKCanvasView // プロパティのフィールドを明示的に指定するの避ける為だけのクラス
    {
        public virtual byte[] ImageBytes { get; set; }
        public virtual byte ImageAlpha { get; set; }
        public virtual bool IsInvalidCanvas { get; set; }
        public virtual bool IsInvalidCenter { get; set; }
        public virtual bool IsClearCanvas { get; set; }
        public virtual bool IsCircle { get; set; }
        public virtual bool EnabledAnimation { get; set; }
    }
    public class AlphaImageView : VoidImage
    {
        const float AntialiasMargin = 0.6f;
        public TimeSpan AnimationSpan = TimeSpan.FromMilliseconds(500);
        public SKRect CanvasRect;
        public float PhysicalPixelRate;
        float ImageRadius;
        SKData ImageData;
        SKBitmap ImageBitmap;
        SKPoint Center;

        public void Dispose()
        {
            ImageBitmap?.Dispose();
            ImageBitmap = null;
            ImageData?.Dispose();
            ImageData = null;
        }

        public override bool IsInvalidCanvas
        {
            set
            {
                if (base.IsInvalidCanvas != value)
                {
                    base.IsInvalidCanvas = value;
                    if (base.IsInvalidCanvas)
                    {
                        Update();
                    }
                }
            }
        }
        public override bool IsInvalidCenter
        {
            set
            {
                if (base.IsInvalidCenter != value)
                {
                    base.IsInvalidCenter = value;
                    if (base.IsInvalidCenter)
                    {
                    	Update();
                    }
                }
            }
        }

        public override Byte[] ImageBytes
        {
            set
            {
                if (base.ImageBytes != value)
                {
                    ImageBitmap?.Dispose();
                    ImageBitmap = null;
                    ImageData?.Dispose();
                    ImageData = null;
                    base.ImageBytes = value;
                    if (null != base.ImageBytes)
                    {
                        ImageData = SKData.CreateCopy(base.ImageBytes);
                        ImageBitmap = SKBitmap.Decode(ImageData);
                    }
                    if (EnabledAnimation)
                    {
                        this.Animate
                        (
                            "ImageAnimation",
                            d => ImageAlpha = (byte)d,
                            0.0,
                            255.0,
                            16,
                            (uint)AnimationSpan.TotalMilliseconds,
                            Easing.SinIn
                        );
                    }
                    else
                    {
                        ImageAlpha = 255;
                    }
                    IsInvalidCenter = true;
                }
            }
        }
        public override byte ImageAlpha
        {
            set
            {
                if (base.ImageAlpha != value)
                {
                    base.ImageAlpha = value;
                    if (null != base.ImageBytes)
                    {
                        IsInvalidCenter = true;
                    }
                }
            }
        }
        public override bool IsCircle
        {
            set
            {
                if (base.IsCircle != value)
                {
                    base.IsCircle = value;
                    IsInvalidCanvas = true;
                }
            }
        }

        public void Update()
        {
            InvalidateSurface();
        }
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            Draw(e.Surface.Canvas);
        }
        public virtual void Draw(SKCanvas Canvas)
        {
            if (IsInvalidCanvas)
            {
                ClearCanvas(Canvas);
            }
            if (IsInvalidCenter)
            {
                DrawCenter(Canvas);
            }
        }
        public virtual void ClearCanvas(SKCanvas Canvas)
        {
            Canvas.GetLocalClipBounds(out CanvasRect);
            PhysicalPixelRate = (float)((CanvasRect.Width + CanvasRect.Height) / (Width + Height));
            ImageRadius = new[]
            {
                CanvasRect.Width,
                CanvasRect.Height
            }.Min() / 2.0f;
            Center = new SKPoint
            (
                CanvasRect.MidX,
                CanvasRect.MidY
            );

            IsInvalidCanvas = false;
            IsInvalidCenter = true;
            //Canvas.Clear();
            Canvas.DrawColor(this.GetRootBackgroudColor(Color.White).ToSKColor());
            IsClearCanvas = true;
        }
        private void DrawCenter(SKCanvas Canvas)
        {
            IsInvalidCenter = false;
            var AntialiasImageRadius = ImageRadius - AntialiasMargin;
            var RootBackgroudColor = this.GetRootBackgroudColor(Color.White);

            //  背景を塗りつぶす
            if (IsCircle && !IsClearCanvas)
            {
                using (var paint = new SKPaint())
                {
                    paint.IsAntialias = true;
                    paint.Color = RootBackgroudColor.ToSKColor();
                    paint.StrokeCap = SKStrokeCap.Round;
                    Canvas.DrawCircle(Center.X, Center.Y, ImageRadius + AntialiasMargin, paint);
                }
            }

            //  イメージの描画
            if (null != ImageBitmap)
            {
                using (var paint = new SKPaint())
                {
                    var Rect = SKRect.Create
                    (
                        Center.X - AntialiasImageRadius,
                        Center.Y - AntialiasImageRadius,
                        AntialiasImageRadius * 2.0f,
                        AntialiasImageRadius * 2.0f
                    );
                    Canvas.DrawBitmap(ImageBitmap, Rect, paint);
                }
                if (ImageAlpha < 255)
                {
                    using (var paint = new SKPaint())
                    {
                        paint.IsAntialias = true;
                        paint.Color = new SKColor((byte)(255*RootBackgroudColor.R), (byte)(255*RootBackgroudColor.G), (byte)(255*RootBackgroudColor.B), (byte)(255 - ImageAlpha));
                        paint.StrokeCap = SKStrokeCap.Round;
                        paint.IsStroke = false;
                        Canvas.DrawCircle(Center.X, Center.Y, AntialiasImageRadius, paint);
                    }
                }
            }

            if (IsCircle && IsClearCanvas)
            {
                using (var paint = new SKPaint())
                {
                    paint.IsAntialias = true;
                    paint.Color = RootBackgroudColor.ToSKColor();
                    paint.StrokeCap = SKStrokeCap.Round;
                    //paint.IsStroke = true;
                    paint.StrokeWidth = PhysicalPixelRate;

                    using (var path = new SKPath())
                    {
                        path.MoveTo(Center + AngleRadiusToPoint(0.0f, AntialiasImageRadius));
                        path.LineTo(CanvasRect.Left, Center.Y);
                        path.LineTo(CanvasRect.Left, CanvasRect.Top);
                        path.LineTo(CanvasRect.Right, CanvasRect.Top);
                        path.LineTo(CanvasRect.Right, Center.Y);
                        path.LineTo(Center + AngleRadiusToPoint(180.0f, AntialiasImageRadius));
                        path.ArcTo(SKRect.Create(Center.X - AntialiasImageRadius, Center.Y - AntialiasImageRadius, AntialiasImageRadius * 2.0f, AntialiasImageRadius * 2.0f), 0.0f, -180.0f, false);
                        path.Close();
                        Canvas.DrawPath(path, paint);
                    }
                    using (var path = new SKPath())
                    {
                        path.MoveTo(Center + AngleRadiusToPoint(180.0f, AntialiasImageRadius));
                        path.LineTo(CanvasRect.Right, Center.Y);
                        path.LineTo(CanvasRect.Right, CanvasRect.Bottom);
                        path.LineTo(CanvasRect.Left, CanvasRect.Bottom);
                        path.LineTo(CanvasRect.Left, Center.Y);
                        path.LineTo(Center + AngleRadiusToPoint(0.0f, AntialiasImageRadius));
                        path.ArcTo(SKRect.Create(Center.X - AntialiasImageRadius, Center.Y - AntialiasImageRadius, AntialiasImageRadius * 2.0f, AntialiasImageRadius * 2.0f), 180.0f, -180.0f, false);
                        path.Close();
                        Canvas.DrawPath(path, paint);
                    }
                }
            }
        }
        public static double DegreeToRadian(double Degree)
        {
            return Degree * Math.PI / 180.0;
        }
        public static SKPoint AngleRadiusToPoint(float Angle, float Radius)
        {
            return new SKPoint
            (
                Radius * (float)Math.Cos(DegreeToRadian(Angle)),
                Radius * (float)Math.Sin(DegreeToRadian(Angle))
            );
        }
    }
}

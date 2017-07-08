//#define DISABLED_SKIASHARP_VIEWS_FORMS
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using System.Threading.Tasks;

using SkiaSharp;
#if !DISABLED_SKIASHARP_VIEWS_FORMS
using SkiaSharp.Views.Forms;
#endif
using System.Diagnostics;

#if DISABLED_SKIASHARP_VIEWS_FORMS
using RuyiJinguBang;
#endif

using keep.grass.Domain;

namespace keep.grass.App
{

#if !DISABLED_SKIASHARP_VIEWS_FORMS
    public class AlphaCircleGraph : SKCanvasView, IAlphaThemeAppliedHandler // プロパティのフィールドを明示的に指定するの避ける為だけのクラス
#else
    public class AlphaCircleGraph : Image, IAlphaThemeAppliedHandler // プロパティのフィールドを明示的に指定するの避ける為だけのクラス
#endif
    {
        public AlphaDrawer Drawer = AlphaDomainFactory.MakeDrawer();
        System.IO.Stream FontSource;
        SKManagedStream FontStream;
        protected SKTypeface Font;

        public AlphaCircleGraph()
        {
            //  ※iOS 版では Font だけ残して他はこの場で Dispose() して構わないが Android 版では遅延処理が行われるようでそれだと disposed object へのアクセスが発生してしまう。
            FontSource = AlphaAppFactory.GetApp().GetFontStream();
            FontStream = new SKManagedStream(FontSource);
            Font = SKTypeface.FromStream(FontStream);
            Drawer.Font = Font;
            Drawer.OnUpdate = d => Update();
        }
        public void Dispose()
        {
            Drawer.Font = null;
            Font?.Dispose();
            Font = null;
            FontStream?.Dispose();
            FontStream = null;
            FontSource?.Dispose();
            FontSource = null;
        }

#if DISABLED_SKIASHARP_VIEWS_FORMS
        public virtual float GetPhysicalPixelRate()
        {
            return 4.0f; // この数字は適当。本来はちゃんとデバイスごとの物理解像度/論理解像度を取得するべき
        }
        public virtual SKColorType GetDeviceColorType()
        {
            return SKColorType.Rgba8888;
        }
#endif
        public void Update()
        {
#if !DISABLED_SKIASHARP_VIEWS_FORMS
            InvalidateSurface();
#else
            //var Radius = (DrawGraphSize / 2.0f) - (Margin * PhysicalPixelRate);
            //var Center = new SKPoint(DrawGraphSize / 2.0f, DrawGraphSize / 2.0f);
            if (0.0 < Width && 0.0 < Height)
            {
                using (var Surface = SKSurface.Create((int)(Width * GetPhysicalPixelRate()), (int)(Height * GetPhysicalPixelRate()), GetDeviceColorType(), SKAlphaType.Premul))
                {
                    Draw(Surface.Canvas);
                    var CanvasImageData = Surface.Snapshot().Encode();
                    Device.BeginInvokeOnMainThread
                    (
                        () =>
                        {
                            Source = ImageSource.FromStream(() => CanvasImageData.AsStream());
                        }
                    );
                }
            }
#endif
        }
#if !DISABLED_SKIASHARP_VIEWS_FORMS
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            Draw(e.Surface.Canvas);
        }
#endif
        public virtual void Draw(SKCanvas Canvas)
        {
            Drawer.Width = Width;
            Drawer.Height = Height;
            Drawer.Draw(Canvas);
        }

        public void AppliedTheme(AlphaTheme Theme)
        {
            Drawer.BackgroundColor = Theme.BackgroundColor;
            Drawer.IsInvalidCanvas = true;
        }
    }
}

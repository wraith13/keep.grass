using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SkiaSharp;
using Xamarin.Forms;
using RuyiJinguBang;
using keep.grass.Domain;

namespace keep.grass.App
{
    public abstract class VoidUserCircleGraph : AlphaCircleGraph // プロパティのフィールドを明示的に指定するの避ける為だけのクラス
    {
        public virtual bool IsActive { get; set; }
        public virtual DateTime Now { get; set; }
        public virtual DateTime LastPublicActivity { get; set; }
        public virtual bool IsVisibleLeftTimeBar { get; set; }
        public virtual bool IsVisibleSatelliteTexts { get; set; }
        public virtual string User { get; set; }
    }
    public class AlphaUserCircleGraph : VoidUserCircleGraph
    {
        AlphaLanguage L = AlphaFactory.MakeSureLanguage();
        AlphaDomain Domain = AlphaFactory.MakeSureDomain();

        public TimeSpan AnimationSpan = TimeSpan.FromMilliseconds(500);

        float LeftTimeBarHeight => Drawer.FontSize * 2.0f;

        public void ApplyTheme(AlphaTheme Theme)
        {
            BackgroundColor = Theme.BackgroundColor.ToColor();
        }

        public override bool IsVisibleLeftTimeBar
        {
            set
            {
                if (base.IsVisibleLeftTimeBar != value)
                {
                    base.IsVisibleLeftTimeBar = value;
                    Drawer.CircleMargin.Top += value ? LeftTimeBarHeight : -LeftTimeBarHeight;
                }
            }
        }

        /*
        public string AltText
        {
            get
            {
                return base.AltText ?? L["unspecified"];
            }
        }
        public override SKColor AltTextColor
        {
            get
            {
                return null == base.AltText ?
					Color.Gray.ToSKColor() :
                    base.AltTextColor;
            }
        }
        */
        public AlphaUserCircleGraph()
        {
            Drawer.AltText = L["unspecified"];
        }

        public override string User
        {
            set
            {
                var Trimed = value?.Trim();
                if (base.User != Trimed)
                {
                    Image = null;
                    base.User = Trimed;
                    Drawer.AltText = User ?? L["unspecified"];
                    Drawer.AltTextColor = null == User ?
                        Color.Gray.ToSKColor() :
						Color.Black.ToSKColor();
                    if (!string.IsNullOrWhiteSpace(User))
                    {
                        new Task
                        (
                            async () =>
                            {
                                var ImageData = await AlphaImageProxy.Get(GitHub.GetIconUrl(User));
                                Xamarin.Forms.Device.BeginInvokeOnMainThread
                                (
                                    () =>
                                    {
                                        Image = ImageData;
                                    }
                                );
                            }
                        ).Start();
                    }
                }
            }
        }
        public byte[] Image
        {
            set
            {
                if (Drawer.Image != value)
                {
                    var HasRequestToAnimation = null == Drawer.Image;
                    Drawer.Image = value;
                    if (HasRequestToAnimation)
                    {
                        if (IsActive)
                        {
                            StartIconAnimation();
                        }
                        else
                        {
                            Drawer.ImageAlpha = 0;
                        }
                    }
                }
            }
        }
        public void StartIconAnimation()
        {
            this.Animate
            (
                "ImageAnimation",
                d => Drawer.ImageAlpha = (byte)d,
                0.0,
                255.0,
                16,
                (uint)AnimationSpan.TotalMilliseconds,
                Easing.SinIn
            );
        }

        public DateTime ActiveAt;
        public TimeSpan ActiveWait = TimeSpan.FromMilliseconds(100);
        public bool IsEarlyActive => DateTime.Now < ActiveAt.Add(ActiveWait);
        public override bool IsActive
        {
            set
            {
                if (base.IsActive != value)
                {
                    base.IsActive = value;
                    if (value)
                    {
                        ActiveAt = DateTime.Now;
                        Task.Delay(ActiveWait.Add(TimeSpan.FromMilliseconds(100)))
                            .ContinueWith
                            (
                                t => Device.BeginInvokeOnMainThread
                                (
                                    () =>
                                    {
                                        StartAnimation();
                                        StartIconAnimation();
                                    }
                                )
                            );
                    }
                }
            }
        }

        private bool HasNextAnimation;

        public void RequestToAnimation()
        {
            if (IsActive)
            {
                if (!IsEarlyActive && !AnimationIsRunning)
                {
                    HasNextAnimation = false;
                    StartAnimation();
                }
                else
                {
                    HasNextAnimation = true;
                }
            }
        }
        private DateTime NewNow;
        public override DateTime Now
        {
            set
            {
                NewNow = value;
                RequestToAnimation();
            }
            get
            {
                return new[]
                {
                    base.Now,
                    DateTime.Now.AddDays(-1),
                }
                .Max();
            }
        }
        private DateTime NewLastPublicActivity;
        public override DateTime LastPublicActivity
        {
            set
            {
                NewLastPublicActivity = value;
                RequestToAnimation();
            }
            get
            {
                return new[]
                {
                    base.LastPublicActivity,
                    DateTime.Now.AddDays(-1),
                }
                .Max();
            }
        }
        public TimeSpan LeftTime => base.LastPublicActivity.AddHours(24) - base.Now;

        public void ResetTime()
        {
            base.Now = default(DateTime);
            base.LastPublicActivity = default(DateTime);
            Drawer.ImageAlpha = 0;
            UpdateSlices();
        }

        public bool AnimationIsRunning =>
                    ((View)this).AnimationIsRunning("NowAnimation") ||
                    ((View)this).AnimationIsRunning("LastPublicActivityAnimation");

        public void StartAnimation()
        {
            var Delta = TimeSpan.FromSeconds(60);
            if (NewLastPublicActivity != LastPublicActivity)
            {
                if (Delta < (NewLastPublicActivity - LastPublicActivity))
                {
                    Debug.WriteLine("Start LastPublicActivity");
                    var AnchorNow = DateTime.Now;
                    this.Animate
                    (
                        "LastPublicActivityAnimation",
                        d =>
                        {
                            base.LastPublicActivity = AnchorNow.Date + TimeSpan.FromMinutes(d);
                            UpdateSlices();
                        },
                        (LastPublicActivity - AnchorNow.Date).TotalMinutes,
                        (NewLastPublicActivity - AnchorNow.Date).TotalMinutes,
                        16,
                        (uint)AnimationSpan.TotalMilliseconds,
                        Easing.SinOut
                    );
                }
                else
                {
                    base.LastPublicActivity = NewLastPublicActivity;
                }
            }
            if (NewNow != Now)
            {
                if (Delta < (NewNow - Now))
                {
                    Debug.WriteLine("Start AnimateNow");
                    var AnchorNow = DateTime.Now;
                    this.Animate
                    (
                        "NowAnimation",
                        d =>
                        {
                            base.Now = AnchorNow.Date + TimeSpan.FromMinutes(d);
                            UpdateSlices();
                        },
                        (Now - AnchorNow.Date).TotalMinutes,
                        (NewNow.AddMilliseconds(1000) - AnchorNow.Date).TotalMinutes,
                        16,
                        (uint)AnimationSpan.TotalMilliseconds,
                        Easing.SinOut
                    );
                }
                else
                {
                    base.Now = NewNow;
                }
            }

            UpdateSlices();
        }

        public override void Draw(SKCanvas Canvas)
        {
            IsActive = true;
            if (HasNextAnimation)
            {
                RequestToAnimation();
            }
            var IsInvalidBar = Drawer.IsInvalidData;
            if (Drawer.IsInvalidCanvas)
            {
                Drawer.ClearCanvas(Canvas);
                UpdateSlices();
            }
            base.Draw(Canvas);
            if (IsVisibleLeftTimeBar && IsInvalidBar)
            {
                DrawLeftTimeBar(Canvas);
            }
        }
        private void DrawLeftTimeBar(SKCanvas Canvas)
        {
            var LeftTimeBarRect = new SKRect
            (
                Drawer.CanvasRect.Left,
                Drawer.CanvasRect.Top,
                Drawer.CanvasRect.Right,
                Drawer.CanvasRect.Top + (LeftTimeBarHeight * Drawer.PhysicalPixelRate)
            );
            using (var paint = new SKPaint())
            {
                if (default(DateTime) != base.LastPublicActivity)
                {
                    paint.Color = AlphaDomain.MakeLeftTimeColor(LeftTime);
                }
                else
                {
                    paint.Color = AlphaDomain.GetElapsedTimeColor();
                }
                Canvas.DrawRect
                (
                    LeftTimeBarRect,
                    paint
                );
            }
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.Color = BackgroundColor.ToSKColor();
                paint.StrokeCap = SKStrokeCap.Round;
                paint.TextSize = Drawer.FontSize * Drawer.PhysicalPixelRate;
                paint.TextAlign = SKTextAlign.Center;
                paint.Typeface = Font;
                var LeftTimeBarText = "";
                if (default(DateTime) != base.LastPublicActivity)
                {
                    LeftTimeBarText = L["Left Time"] + " : " + Domain.ToString(LeftTime);
                }
                else
                if (string.IsNullOrWhiteSpace(User))
                {
                    LeftTimeBarText = L["Please specify GitHub user."];
                }
                Canvas.DrawText
                (
                    LeftTimeBarText,
                    LeftTimeBarRect.MidX,
                    LeftTimeBarRect.MidY + (paint.TextSize / 2.0f),
                    paint
                );
                paint.Typeface = null;
            }
        }

        public void UpdateSlices()
        {
            Drawer.SetStartAngle(AlphaDomain.TimeToAngle(Now));
            if (default(DateTime) != base.LastPublicActivity)
            {
                var Today = Now.Date;
                var LeftTimeColor = AlphaDomain.MakeLeftTimeColor(LeftTime);
                Drawer.AltTextColor = LeftTimeColor;
                Drawer.Data = AlphaDomain.MakeSlices(LeftTime, LeftTimeColor);

                if (!IsVisibleSatelliteTexts || Drawer.GraphSize < Drawer.FontSize * 6.0f)
                {
                    Drawer.SatelliteTexts = null;
                }
                else
                if (Drawer.GraphSize < Drawer.FontSize * 9.0f)
                {
                    Drawer.SatelliteTexts = AlphaDomain.MakeSatelliteTexts(Now, LastPublicActivity, 6);
                }
                else
                if (Drawer.GraphSize < Drawer.FontSize * 12.0f)
                {
                    Drawer.SatelliteTexts = AlphaDomain.MakeSatelliteTexts(Now, LastPublicActivity, 3);
                }
                else
                {
                    Drawer.SatelliteTexts = AlphaDomain.MakeSatelliteTexts(Now, LastPublicActivity);
                }

                //Task.Run(() => Domain.AutoUpdateLastPublicActivityAsync());
            }
            else
            {
                Drawer.AltTextColor = Color.Gray.ToSKColor();
                Drawer.Data = AlphaDomain.MakeSlices(TimeSpan.Zero, Color.Lime.ToSKColor());
                if (!IsVisibleSatelliteTexts)
                {
                    Drawer.SatelliteTexts = null;
                }
                else
                {
                    Drawer.SatelliteTexts = Enumerable.Range(0, 24).Select
                    (
                        i => new CircleGraphSatelliteText
                        {
                            Text = i.ToString(),
	                        Color = Color.Gray.ToSKColor(),
                            Angle = 360.0f * ((float)(i) / 24.0f),
                        }
                    );
                }
            }
        }
    }
}

using System;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using ImageCircle.Forms.Plugin.Abstractions;
using RuyiJinguBang;

namespace keep.grass
{
    public class AlphaCircleImageCell : ViewCell
    {
        public static readonly BindableProperty ImageBytesProperty = typeof(AlphaCircleImageCell).GetRuntimeProperty("ImageBytes").CreateBindableProperty();
        public static readonly BindableProperty TextProperty = typeof(AlphaCircleImageCell).GetRuntimeProperty("Text").CreateBindableProperty();

        protected AlphaImageView Image = AlphaFactory.MakeCircleImage();
        protected Label TextLabel = new Label();
        protected AlphaImageView OptionImage = new AlphaImageView();
        protected StackLayout Stack;

        public AlphaCircleImageCell() : base()
        {
            View = new Grid().SetSingleChild
            (
                Stack = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.Center,
                    Padding = new Thickness(20, 2, 0, 2),
                    Children =
                    {
                        Image,
                        TextLabel,
                        OptionImage,
                    },
                }
            );

            Image.EnabledAnimation = true;
            Image.IsVisible = null != Image.ImageBytes;
            Image.VerticalOptions = LayoutOptions.Center;
            TextLabel.VerticalOptions = LayoutOptions.Center;
            TextLabel.HorizontalOptions = LayoutOptions.StartAndExpand;
            OptionImage.VerticalOptions = LayoutOptions.Center;
            OptionImage.HorizontalOptions = LayoutOptions.End;
            OptionImage.ImageBytes = AlphaFactory.GetApp().GetRightImageSource();
            OptionImage.IsVisible = null != CommandValue;
            AlphaTheme.Apply(this);
        }

        private Command CommandValue = null;
        public Command Command
        {
            set
            {
                CommandValue = value;
                OptionImage.IsVisible = null != CommandValue && null != OptionImage.ImageBytes;
            }
            get
            {
                return CommandValue;
            }
        }
        protected override void OnTapped()
        {
            base.OnTapped();
            if (null != CommandValue)
            {
                CommandValue.Execute(this);
            }
        }

        public byte[] ImageBytes
        {
            get
            {
                return Image.ImageBytes;
            }
            set
            {
                Image.ImageBytes = value;
                Image.IsVisible = null != Image.ImageBytes;
            }
        }
        public string ImageSourceUrl
        {
            set
            {
                AlphaImageProxy.Get(value)
                    .ContiuneWithOnUIThread(t => ImageBytes = t.Result);
            }
        }

        public String Text
        {
            get
            {
                return TextLabel.Text;
            }
            set
            {
                TextLabel.Text = value;
            }
        }
        public Color TextColor
        {
            get
            {
                return TextLabel.TextColor;
            }
            set
            {
                TextLabel.TextColor = value;
            }
        }
        public Color BackgroundColor
        {
            get
            {
                return View.BackgroundColor;
            }
            set
            {
                View.BackgroundColor = value;
                Stack.BackgroundColor = value;
                Image.BackgroundColor = value;
                TextLabel.BackgroundColor = value;
                OptionImage.BackgroundColor = value;
            }
        }
        public byte[] OptionImageBytes
        {
            get
            {
                return OptionImage.ImageBytes;
            }
            set
            {
                OptionImage.ImageBytes = value;
                OptionImage.IsVisible = null != CommandValue && null != OptionImage.ImageBytes;
            }
        }
    }
}


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
        public static readonly BindableProperty ImageSourceProperty = typeof(AlphaCircleImageCell).GetRuntimeProperty("ImageSource").CreateBindableProperty();
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
            OptionImage.ImageSource = AlphaFactory.GetApp().GetRightImageSource();
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
        public ImageSource ImageSource
        {
            get
            {
                return Image.ImageSource;
            }
            set
            {
                Image.ImageSource = value;
                Image.IsVisible = null != Image.ImageSource;
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
        public ImageSource OptionImageSource
        {
            get
            {
                return OptionImage.ImageSource;
            }
            set
            {
                OptionImage.ImageSource = value;
                OptionImage.IsVisible = null != CommandValue && null != OptionImage.ImageSource;
            }
        }
    }
}


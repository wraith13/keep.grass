using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using RuyiJinguBang;

namespace keep.grass.App
{
    public interface VoidSwitchCell
    {
        string Text { get; set; }
        bool On { get; set; }
        Cell AsCell();
    }

    //  Xamarin.Forms 標準の SwitchCell だと TextColor が指定できないので自前実装
    public class AlphaSwitchCell : ViewCell, VoidSwitchCell
    {
        protected Label TextLabel = new Label();
        protected Switch Switch = new Switch();

        public AlphaSwitchCell() : base()
        {
            View = new Grid().SetSingleChild
            (
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.Center,
                    Padding = new Thickness(20, 2, 0, 2),
                    Children =
                    {
                        TextLabel,
                        Switch,
                    },
                }
            );

            TextLabel.VerticalOptions = LayoutOptions.Center;
            TextLabel.HorizontalOptions = LayoutOptions.StartAndExpand;
            Switch.VerticalOptions = LayoutOptions.Center;
            Switch.HorizontalOptions = LayoutOptions.End;
            AlphaTheme.Apply(this);
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

        public new string Text
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
        public new bool On
        {
            get
            {
                return Switch.IsToggled;
            }
            set
            {
                Switch.IsToggled = value;
            }
        }
        public Cell AsCell()
        {
            return this;
        }
    }
}

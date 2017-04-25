using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using RuyiJinguBang;

namespace keep.grass.UWP
{
    class OmegaSwitchCell :ViewCell, VoidSwitchCell
    {
        protected Label TextLabel = new Label();
        protected Switch OnSwitch = new Switch();

        public string Text
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
        public bool On
        {
            get
            {
                return OnSwitch.IsToggled;
            }
            set
            {
                OnSwitch.IsToggled = value;
            }
        }
        public Cell AsCell()
        {
            return this;
        }

        public OmegaSwitchCell() : base()
		{
            View = new Grid().SetSingleChild
            (
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.Center,
                    Padding = new Thickness(20, 0, 10, 0),
                    Children =
                    {
                        TextLabel,
                        OnSwitch,
                    },
                }
            );

            TextLabel.HorizontalOptions = LayoutOptions.StartAndExpand;
            TextLabel.VerticalOptions = LayoutOptions.Center;
            OnSwitch.HorizontalOptions = LayoutOptions.End;
            OnSwitch.VerticalOptions = LayoutOptions.Center;
        }
    }
}

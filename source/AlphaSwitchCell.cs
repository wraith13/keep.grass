using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace keep.grass
{
    public interface VoidSwitchCell
    {
        string Text { get; set; }
        bool On { get; set; }
        Cell AsCell();
    }
    public class AlphaSwitchCell : SwitchCell, VoidSwitchCell
    {
        public new string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }
        public new bool On
        {
            get
            {
                return base.On;
            }
            set
            {
                base.On = value;
            }
        }
        public Cell AsCell()
        {
            return this;
        }
    }
}

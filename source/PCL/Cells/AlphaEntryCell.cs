using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace keep.grass
{
    public interface VoidEntryCell
    {
        string Label { get; set; }
        string Text { get; set; }
        Cell AsCell();
    }
    public class AlphaEntryCell : EntryCell, VoidEntryCell
    {
        public new string Label
        {
            get
            {
                return base.Label;
            }
            set
            {
                base.Label = value;
            }
        }
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
        public Cell AsCell()
        {
            return this;
        }
    }
}

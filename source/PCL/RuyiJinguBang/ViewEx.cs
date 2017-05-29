using System;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RuyiJinguBang
{
    public static class ViewEx
    {
        public static Color GetRootBackgroudColor(this VisualElement Self, Color DefaultColor)
        {
            if (Color.Default != Self.BackgroundColor)
            {
                return Self.BackgroundColor;
            }
            var Parent = Self.Parent as VisualElement;
            if (null != Parent)
            {
                return Parent.GetRootBackgroudColor(DefaultColor);
            }
            return DefaultColor;
        }
        public static Color GetRootBackgroudColor(this VisualElement Self)
        {
            return Self.GetRootBackgroudColor(Color.Default);
        }
    }
}

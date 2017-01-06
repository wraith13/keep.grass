using System;
using System.Reflection;
using Xamarin.Forms;

namespace keep.grass
{
	public static class BindablePropertyEx
	{
		public static BindableProperty CreateBindableProperty(this PropertyInfo PropertyInfo, object DefaultValue = null)
		{
			return BindableProperty.Create
			(
				PropertyInfo.Name,
				PropertyInfo.PropertyType,
				PropertyInfo.DeclaringType,
				DefaultValue,
				propertyChanged: (bindable, oldValue, newValue) => PropertyInfo.SetValue(bindable, newValue)
			);
		}
	}
}

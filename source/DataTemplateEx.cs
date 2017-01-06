using System;
using System.Reflection;
using Xamarin.Forms;

namespace keep.grass
{
	public class DataTemplateEx : DataTemplate
	{
		Type DeclaringType;

		public DataTemplateEx(Type aDeclaringType)
			:base(aDeclaringType)
		{
			DeclaringType = aDeclaringType;
		}
		public DataTemplateEx SetBinding(string ViewPropertyName, string DataPropertyName, object DefaultValue = null)
		{
			this.SetBinding
			(
				DeclaringType.GetRuntimeProperty(ViewPropertyName).CreateBindableProperty(DefaultValue),
				DataPropertyName
			);
			return this;
		}
	}
}

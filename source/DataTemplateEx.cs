using System;
using System.Linq;
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
		public DataTemplateEx SetBinding(string ViewPropertyName, string DataPropertyName, object DefaultValue)
		{
			this.SetBinding
			(
				DeclaringType.GetRuntimeProperty(ViewPropertyName).CreateBindableProperty(DefaultValue),
				DataPropertyName
			);
			return this;
		}
        public DataTemplateEx SetBinding(string ViewPropertyName, string DataPropertyName)
        {
            this.SetBinding
            (
                DeclaringType.GetRuntimeProperty(ViewPropertyName).CreateBindableProperty(),
                DataPropertyName
            );
            return this;
        }
		public DataTemplateEx SetBindingList(params string[] PropertyNameList)
		{
			foreach(var PropertyName in PropertyNameList)
			{
                SetBinding
                (
                    PropertyName,
                    PropertyName
                );
			}
			return this;
		}
	}
}

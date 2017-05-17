using System;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace RuyiJinguBang
{
    public class DataTemplateEx : DataTemplate
    {
        Type DeclaringType;

        public DataTemplateEx(Type aDeclaringType)
            : base(aDeclaringType)
        {
            DeclaringType = aDeclaringType;
            System.Diagnostics.Debug.WriteLine($"DeclaringType:{DeclaringType.FullName}");
        }
        public PropertyInfo GetRuntimeProperty(string ViewPropertyName)
        {
            var result = DeclaringType.GetRuntimeProperty(ViewPropertyName);
            if (null == result)
            {
                //  Android版のデバッグビルドにおいて Reflection が腐っててプロパティー情報が正常に取得できない問題が確認されている。
                foreach (var i in DeclaringType.GetRuntimeProperties())
                {
                    System.Diagnostics.Debug.WriteLine($"i.Name:{i.Name}");
                }
                throw new ArgumentOutOfRangeException("ViewPropertyName", $"{DeclaringType.FullName} has not '{ViewPropertyName}' property. If this is a debug build of the Android version it is highly likely that it is a bug in Xamarin.");
            }
            return result;
        }
        public DataTemplateEx SetBinding(string ViewPropertyName, string DataPropertyName, object DefaultValue)
        {
            this.SetBinding
            (
                GetRuntimeProperty(ViewPropertyName).CreateBindableProperty(DefaultValue),
                DataPropertyName
            );
            return this;
        }
        public DataTemplateEx SetBinding(string ViewPropertyName, string DataPropertyName)
        {
            this.SetBinding
            (
                GetRuntimeProperty(ViewPropertyName).CreateBindableProperty(),
                DataPropertyName
            );
            return this;
        }
        public DataTemplateEx SetBindingList(params string[] PropertyNameList)
        {
            foreach (var PropertyName in PropertyNameList)
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

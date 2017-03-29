using System;
using System.Reflection;

namespace keep.grass
{
	static public class ValueEx
	{
		public static bool IsDefault<T>(this T value) where T : IEquatable<T>
		{
			return default(T).Equals(value);
		}
        public static T GetValue<T>(this object o, string name)
        {
            return (T)o.GetType().GetRuntimeProperty(name).GetValue(o);
        }
    }
}


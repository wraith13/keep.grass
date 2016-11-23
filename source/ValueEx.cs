using System;

namespace keep.grass
{
	static public class ValueEx
	{
		public static bool IsDefault<T>(this T value) where T : IEquatable<T>
		{
			return default(T).Equals(value);
		}
	}
}


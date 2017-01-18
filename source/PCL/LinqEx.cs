using System;
using System.Collections.Generic;
using System.Linq;

namespace keep.grass
{
	static public class LinqEx
	{
		public static int IndexOf<T>(this IEnumerable<T> list, T value)
		{
			return list
				.Select
				(
					(i, index) => new
					{
						value = i,
						index = index,
					}
				)
				.Where(i => i.value.Equals(value))
				.Select(i => i.index)
				.FirstOrDefault(-1);
		}
		public static T FirstOrDefault<T>(this IEnumerable<T> list, T DefaultValue)
		{
			return list
				.Concat(DefaultValue)
				.First();
		}
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> list, T Value)
        {
            return list
                .Concat(new[] { Value });
        }
	}
}


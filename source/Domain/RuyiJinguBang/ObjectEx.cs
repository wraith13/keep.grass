using System;

namespace RuyiJinguBang
{
    static public class ObjectEx
    {
        public static void Using<T>(this T self, Action<T> func) where T : IDisposable
        {
            using(self)
            {
                func(self);
            }
        }
        public static U Using<T, U>(this T self, Func<T, U> func) where T : IDisposable
        {
            using (self)
            {
                return func(self);
            }
        }
    }
}


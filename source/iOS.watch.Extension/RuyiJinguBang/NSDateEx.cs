using System;
using Foundation;

namespace RuyiJinguBang
{
    public static class NSDateEx
    {
        public static DateTime ToDateTime(this NSDate source)
        {
            return new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(source.SecondsSinceReferenceDate);
        }
    }
}

using System;
using System.Threading.Tasks;

namespace RuyiJinguBang
{
    public static class TaskEx
    {
        public static void LeavingThrown(this Task Task)
        {
            //  なにもしない。タスクを投げっぱなしにすることのその明示と警告除けの為の拡張メソッド
        }
        public static Task ContiuneWithOnUIThread<T>(this Task<T> Self, Action<Task<T>> Receiver)
        {
            return Self.ContinueWith
            (
                t => Xamarin.Forms.Device.BeginInvokeOnMainThread(() => Receiver(t))
            );
        }
    }
}

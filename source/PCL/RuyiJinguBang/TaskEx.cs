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
    }
}

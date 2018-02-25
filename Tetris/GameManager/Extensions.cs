using System;
using System.Collections.Generic;

namespace TetrisGameManager
{
    internal static class Extensions
    {
        /// <summary>
        /// Same as good ol' ForEach, but this time with return!
        /// </summary>
        public static List<T> ForEachWithReturn<T>(this List<T> list, Action<T> a)
        {
            list.ForEach(a);
            return list;
        }
    }
}

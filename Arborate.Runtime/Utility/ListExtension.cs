using System;
using System.Collections.Generic;
using System.Text;

namespace Arborate.Runtime.Utility
{
    internal static class ListExtension
    {
        internal static T Pop<T>(this List<T> list)
        {
            int lastElement = list.Count - 1;
            T returnValue = list[lastElement];
            list.RemoveAt(lastElement);
            return returnValue;
        }

        internal static void Push<T>(this List<T> list, T element)
        {
            list.Add(element);
        }
    }
}

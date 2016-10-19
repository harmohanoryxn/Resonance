using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cloudmaster.WCS.WebApps.Cleaning.Extensions
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }
    }
}
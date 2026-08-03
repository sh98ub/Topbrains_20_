using System;
using System.Collections.Generic;
using System.Linq;

namespace Q42_ExtensionMethod
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            HashSet<TKey> seen = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (element != null && seen.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            string[] items = { "1:Alice", "2:Bob", "1:Charlie", "3:David", "2:Eve" };

            string[] distinctNames = GetDistinctNames(items);

            Console.WriteLine($"Input:  [{string.Join(", ", items)}]");
            Console.WriteLine($"Output: [{string.Join(", ", distinctNames)}]");
        }

        public static string[] GetDistinctNames(string[] items)
        {
            if (items == null || items.Length == 0)
                return Array.Empty<string>();

            return items
                .DistinctBy(item => item.Split(':')[0])
                .Select(item => item.Split(':')[1])
                .ToArray();
        }
    }
}

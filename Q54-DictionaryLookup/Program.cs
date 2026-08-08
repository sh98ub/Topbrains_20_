using System;
using System.Collections.Generic;

namespace Q54_DictionaryLookup
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var salaryDict = new Dictionary<int, long>
            {
                { 1, 20000 },
                { 4, 40000 },
                { 5, 15000 }
            };

            int[] ids = { 1, 4, 5 };

            long totalSalary = CalculateTotalSalary(ids, salaryDict);
            Console.WriteLine(totalSalary);
        }

        public static long CalculateTotalSalary(IEnumerable<int> ids, Dictionary<int, long> salaryDict)
        {
            if (ids == null || salaryDict == null) return 0;

            long total = 0;
            foreach (int id in ids)
            {
                if (salaryDict.TryGetValue(id, out long salary))
                {
                    total += salary;
                }
            }
            return total;
        }
    }
}

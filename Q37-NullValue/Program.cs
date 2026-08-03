using System;
using System.Linq;

namespace Q37_NullValue
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double?[] values = { 10.5, null, 20.25, 30.75, null };
            double? avg = CalculateAverage(values);

            Console.WriteLine(avg.HasValue ? avg.Value.ToString("F2") : "null");

            double?[] emptyValues = { null, null };
            double? emptyAvg = CalculateAverage(emptyValues);
            Console.WriteLine(emptyAvg.HasValue ? emptyAvg.Value.ToString("F2") : "null");
        }

        public static double? CalculateAverage(double?[] values)
        {
            if (values == null || values.Length == 0)
                return null;

            double sum = 0;
            long count = 0;

            foreach (var val in values)
            {
                if (val.HasValue)
                {
                    sum += val.Value;
                    count++;
                }
            }

            if (count == 0)
                return null;

            double avg = sum / count;
            return Math.Round(avg, 2, MidpointRounding.AwayFromZero);
        }
    }
}

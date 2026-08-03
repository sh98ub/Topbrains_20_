using System;

namespace Q43_TimeConversion
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] testValues = { 125, 60, 0, 45, 3605 };

            foreach (int sec in testValues)
            {
                Console.WriteLine($"{sec} -> \"{FormatSeconds(sec)}\"");
            }
        }

        public static string FormatSeconds(int totalSeconds)
        {
            if (totalSeconds < 0) return "0:00";

            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            return $"{minutes}:{seconds:D2}";
        }
    }
}

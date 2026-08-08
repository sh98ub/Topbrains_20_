using System;

namespace Q53_FeetToCentimeters
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] testFeet = { 5, 10, 1, 0, 100 };

            foreach (int ft in testFeet)
            {
                double cm = FeetToCentimeters(ft);
                Console.WriteLine($"{ft} feet = {cm:F2} cm");
            }
        }

        public static double FeetToCentimeters(int feet)
        {
            if (feet < 0) return 0.0;

            double centimeters = feet * 30.48;
            return Math.Round(centimeters, 2, MidpointRounding.AwayFromZero);
        }
    }
}

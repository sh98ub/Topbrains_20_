using System;

namespace Q14_GreatestCommonDivisor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter a: ");
            int a = int.Parse(Console.ReadLine()!);

            Console.Write("Enter b: ");
            int b = int.Parse(Console.ReadLine()!);

            int result = Gcd(a, b);
            Console.WriteLine($"GCD: {result}");
        }

        public static int Gcd(int a, int b)
        {
            if (b == 0) return a;
            return Gcd(b, a % b);
        }
    }
}

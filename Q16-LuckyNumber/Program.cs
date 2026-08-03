using System;

namespace Q16_LuckyNumber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine()!;
            if (string.IsNullOrWhiteSpace(input)) return;

            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) return;

            long m = long.Parse(parts[0]);
            long n = long.Parse(parts[1]);

            int count = CountLuckyNumbers(m, n);
            Console.WriteLine(count);
        }

        public static int CountLuckyNumbers(long m, long n)
        {
            int count = 0;
            for (long i = m; i <= n; i++)
            {
                if (IsLuckyNumber(i))
                {
                    count++;
                }
            }
            return count;
        }

        public static bool IsLuckyNumber(long x)
        {
            if (x <= 0 || IsPrime(x))
                return false;

            long sumX = SumOfDigits(x);
            long squareX = x * x;
            long sumSquareX = SumOfDigits(squareX);

            return sumSquareX == (sumX * sumX);
        }

        public static bool IsPrime(long n)
        {
            if (n < 2) return false;
            if (n == 2 || n == 3) return true;
            if (n % 2 == 0 || n % 3 == 0) return false;

            for (long i = 5; i * i <= n; i += 6)
            {
                if (n % i == 0 || n % (i + 2) == 0)
                    return false;
            }
            return true;
        }

        public static long SumOfDigits(long n)
        {
            long sum = 0;
            n = Math.Abs(n);
            while (n > 0)
            {
                sum += n % 10;
                n /= 10;
            }
            return sum;
        }
    }
}

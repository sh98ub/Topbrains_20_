using System;

namespace Q39_Parsing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] tokens = { "10", "abc", "20", "2147483648", "-5" };
            int sum = SumValidIntegers(tokens);

            Console.WriteLine($"Input: [{string.Join(", ", tokens)}]");
            Console.WriteLine($"Sum:   {sum}");
        }

        public static int SumValidIntegers(string[] tokens)
        {
            if (tokens == null || tokens.Length == 0) return 0;

            int sum = 0;
            foreach (var token in tokens)
            {
                if (int.TryParse(token, out int value))
                {
                    unchecked
                    {
                        sum += value;
                    }
                }
            }
            return sum;
        }
    }
}

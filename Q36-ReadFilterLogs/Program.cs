using System;
using System.IO;
using System.Linq;

namespace Q36_ReadFilterLogs
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string inputFile = "log.txt";
            string outputFile = "error.txt";

            if (!File.Exists(inputFile))
            {
                File.WriteAllLines(inputFile, new string[]
                {
                    "2026-08-08 10:00:01 INFO Application started",
                    "2026-08-08 10:00:05 WARN High memory usage detected",
                    "2026-08-08 10:00:10 ERROR Failed to connect to database",
                    "2026-08-08 10:00:15 INFO Retrying database connection",
                    "2026-08-08 10:00:20 ERROR Connection timeout exceeded"
                });
            }

            FilterErrorLogs(inputFile, outputFile);

            Console.WriteLine($"Extracted ERROR logs from {inputFile} to {outputFile}:");
            if (File.Exists(outputFile))
            {
                Console.WriteLine(File.ReadAllText(outputFile));
            }
        }

        public static void FilterErrorLogs(string inputPath, string outputPath)
        {
            if (!File.Exists(inputPath)) return;

            var errorLogs = File.ReadLines(inputPath)
                .Where(line => line.Contains("ERROR", StringComparison.OrdinalIgnoreCase))
                .ToList();

            File.WriteAllLines(outputPath, errorLogs);
        }
    }
}

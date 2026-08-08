using System;
using System.Collections.Generic;
using System.Globalization;

namespace Q37_TotalAreaShapes
{
    public interface IArea
    {
        double GetArea();
    }

    public abstract class Shape : IArea
    {
        public abstract double GetArea();
    }

    public class Circle : Shape
    {
        public double Radius { get; set; }

        public Circle(double radius)
        {
            Radius = radius;
        }

        public override double GetArea()
        {
            return Math.PI * Radius * Radius;
        }
    }

    public class Rectangle : Shape
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public Rectangle(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public override double GetArea()
        {
            return Width * Height;
        }
    }

    public class Triangle : Shape
    {
        public double Base { get; set; }
        public double Height { get; set; }

        public Triangle(double @base, double height)
        {
            Base = @base;
            Height = height;
        }

        public override double GetArea()
        {
            return 0.5 * Base * Height;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            string[] shapesInput = {
                "C 5",
                "R 4 10",
                "T 6 8"
            };

            double totalArea = CalculateTotalArea(shapesInput);
            Console.WriteLine($"Total Area: {totalArea}");
        }

        public static double CalculateTotalArea(string[] shapes)
        {
            if (shapes == null || shapes.Length == 0)
                return 0.0;

            double sum = 0.0;

            foreach (var shapeStr in shapes)
            {
                if (string.IsNullOrWhiteSpace(shapeStr)) continue;

                string[] parts = shapeStr.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2) continue;

                string type = parts[0].ToUpper();
                Shape? shape = null;

                if (type == "C" && double.TryParse(parts[1], CultureInfo.InvariantCulture, out double r))
                {
                    shape = new Circle(r);
                }
                else if (type == "R" && parts.Length >= 3 &&
                         double.TryParse(parts[1], CultureInfo.InvariantCulture, out double w) &&
                         double.TryParse(parts[2], CultureInfo.InvariantCulture, out double h))
                {
                    shape = new Rectangle(w, h);
                }
                else if (type == "T" && parts.Length >= 3 &&
                         double.TryParse(parts[1], CultureInfo.InvariantCulture, out double b) &&
                         double.TryParse(parts[2], CultureInfo.InvariantCulture, out double th))
                {
                    shape = new Triangle(b, th);
                }

                if (shape != null)
                {
                    sum += shape.GetArea();
                }
            }

            return Math.Round(sum, 2, MidpointRounding.AwayFromZero);
        }
    }
}

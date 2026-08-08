using System;
using System.Collections.Generic;

namespace Q60_CustomSortingIComparer
{
    public class Student
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public double Marks { get; set; }

        public Student(string name, int age, double marks)
        {
            Name = name;
            Age = age;
            Marks = marks;
        }

        public override string ToString()
        {
            return $"{Name} (Marks: {Marks}, Age: {Age})";
        }
    }

    public class StudentComparer : IComparer<Student>
    {
        public int Compare(Student? x, Student? y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return 1;
            if (y == null) return -1;

            int marksCompare = y.Marks.CompareTo(x.Marks);
            if (marksCompare != 0) return marksCompare;

            int ageCompare = x.Age.CompareTo(y.Age);
            if (ageCompare != 0) return ageCompare;

            return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            List<Student> students = new List<Student>
            {
                new Student("Alice", 20, 85.5),
                new Student("Bob", 18, 90.0),
                new Student("Charlie", 19, 85.5),
                new Student("David", 22, 90.0)
            };

            students.Sort(new StudentComparer());

            foreach (var student in students)
            {
                Console.WriteLine(student);
            }
        }
    }
}

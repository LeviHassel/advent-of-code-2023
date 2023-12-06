using System.Reflection;

namespace AdventOfCode2023;

static class Program
{
    static void Main()
    {
        Console.WriteLine("What would you like to run?");
        Console.WriteLine("Please enter a string in the following format: \"Day.Part.Input/Sample\"");
        Console.WriteLine("Examples: \"3.1.i\", \"5.2.s\"");
        
        string? selection = Console.ReadLine();
        var selectionParts = selection?.Split('.');
        var day = selectionParts?[0];
        var part = selectionParts?[1];
        var inputType = selectionParts?[2];

        string className = $"AdventOfCode2023.Day{day}.Part{part}";
        Type? type = Type.GetType(className);

        if (type != null)
        {
            MethodInfo? method = type.GetMethod("Run");

            if (method != null && method.IsStatic)
            {
                var projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent;
                var inputFile = inputType == "s" ? "Sample.txt" : "Input.txt";
                var inputFilePath = $"{projectDirectory}/Day{day}/{inputFile}";
                var lines = File.ReadLines(inputFilePath).ToArray();
                
                Console.WriteLine($"Running Day {day}, Part {part} on {inputFilePath}...");
                Console.WriteLine();

                method.Invoke(null, new object[] { lines });
            }
            else
            {
                Console.WriteLine("Invalid input data.");
            }
        }
        else
        {
            Console.WriteLine("Invalid input data.");
        }
    }
}
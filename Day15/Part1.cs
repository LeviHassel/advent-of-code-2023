namespace AdventOfCode2023.Day15;

// https://adventofcode.com/2023/day/15

internal static class Part1
{
    public static void Run(string[] lines)
    {
        var inputs = lines.First().Split(',');
        
        var sumOfValues = 0;
        
        foreach (var input in inputs)
        {
            var inputValue = RunHashAlgorithm(input);
            sumOfValues += inputValue;
        }

        Console.WriteLine($"Sum of results: {sumOfValues}");
    }

    public static int RunHashAlgorithm(string input)
    {
        var stringValue = 0;

        foreach (var character in input)
        {
            // Determine the ASCII code for the current character of the string
            var value = (int)character;

            // Increase the current value by the ASCII code you just determined.
            stringValue += value;

            // Set the current value to itself multiplied by 17.
            stringValue *= 17;
            
            // Set the current value to the remainder of dividing itself by 256.
            stringValue = stringValue % 256;
        }

        return stringValue;
    }
}
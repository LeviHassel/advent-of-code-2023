namespace AdventOfCode2023.Day15;

// https://adventofcode.com/2023/day/15

internal static class Part2
{
    public static void Run(string[] lines)
    {
        // Create boxes
        var boxes = new Dictionary<int, List<Lens>>();

        for (var i=0; i < 256; i++)
        {
            boxes.Add(i, new List<Lens>());
        }
        
        var inputs = lines.First().Split(',');

        foreach (var input in inputs)
        {
            var lastCharacter = input.Last();

            if (Char.IsDigit(lastCharacter))
            {
                var contents = input.Split('=');

                var lens = new Lens()
                {
                    Label = contents[0],
                    FocalLength = int.Parse(contents[1])
                };
                
                var boxNumber = RunHashAlgorithm(lens.Label);
                var box = boxes[boxNumber];

                var matchingLensInBox = box.FindIndex( l => l.Label == lens.Label);

                if (matchingLensInBox != -1)
                {
                    box.RemoveAll(l => l.Label == lens.Label);
                    box.Insert(matchingLensInBox, lens);
                }
                else
                {
                    box.Add(lens);
                }
            }
            else if (lastCharacter == '-')
            {
                var labelToRemove = input.Split('-')[0];
                
                var boxNumber = RunHashAlgorithm(labelToRemove);
                var box = boxes[boxNumber];
                
                box.RemoveAll(l => l.Label == labelToRemove);
            }
        }

        var totalLensFocusingPower = 0;
        
        foreach (var box in boxes)
        {
            for (var i = 0; i < box.Value.Count; i++)
            {
                var lens = box.Value[i];
                var lensFocusingPower = (1 + box.Key) * (i + 1) * lens.FocalLength;
                totalLensFocusingPower += lensFocusingPower;
            }
        }

        Console.WriteLine($"Lens focusing power: {totalLensFocusingPower}");
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

class Lens
{
    public string Label;
    public int FocalLength;
}
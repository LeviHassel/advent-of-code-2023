namespace AdventOfCode2023.Day2;

// ReSharper disable once UnusedType.Global
internal static class Part2
{
    public static void Run(string[] lines)
    {
        // cube can be red, blue or green
        
        // which games would have been possible if the bag contained only 12 red cubes, 13 green cubes, and 14 blue cubes?
        
        // Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green

        var powerSum = 0;

        foreach (var line in lines)
        {
            var maxRedCount = 0;
            var maxGreenCount = 0;
            var maxBlueCount = 0;
            
            var titleContentsSplit = line.Split(':');
            var title = titleContentsSplit.First();
            var gameIdString = title.Replace("Game ", "");
            
            var content = titleContentsSplit.Last();
            
            var sets = content.Split(';');
            
            foreach (var set in sets)
            {
                var redCount = 0;
                var greenCount = 0;
                var blueCount = 0;
                
                var colors = set.Split(',');

                foreach (var color in colors)
                {
                    var trimmedColor = color.Trim();
                    var details = trimmedColor.Split(' ');
                    var colorCount = details[0];
                    var colorName = details[1];

                    switch (colorName)
                    {
                        case "red":
                            redCount = int.Parse(colorCount);
                            break;
                        case "green":
                            greenCount = int.Parse(colorCount);
                            break;
                        case "blue":
                            blueCount = int.Parse(colorCount);
                            break;
                        default:
                            Console.WriteLine("Color name did not match");
                            break;
                    }
                }
                
                if (redCount > maxRedCount)
                {
                    maxRedCount = redCount;
                }
                
                if (blueCount > maxBlueCount)
                {
                    maxBlueCount = blueCount;
                }
                
                if (greenCount > maxGreenCount)
                {
                    maxGreenCount = greenCount;
                }
            }

            powerSum += (maxRedCount * maxBlueCount * maxGreenCount);
            
            Console.WriteLine(powerSum);
        }
        
    }
}
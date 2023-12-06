namespace AdventOfCode2023.Day2;

// https://adventofcode.com/2023/day/2

internal static class Part1
{
    public static void Run(string[] lines)
    {
        var maxRedCount = 12;
        var maxGreenCount = 13;
        var maxBlueCount = 14;

        var possibleGameIdSum = 0;

        foreach (var line in lines)
        {
            var titleContentsSplit = line.Split(':');
            var title = titleContentsSplit.First();
            var gameIdString = title.Replace("Game ", "");
            var gameId = int.Parse(gameIdString);
            
            var content = titleContentsSplit.Last();
            
            var sets = content.Split(';');
            
            var isPossible = true;

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
                
                if (redCount > maxRedCount || greenCount > maxGreenCount || blueCount > maxBlueCount)
                {
                    isPossible = false;
                }
            }

            if (isPossible)
            {
                possibleGameIdSum += gameId;
            }
            
            Console.WriteLine(possibleGameIdSum);
        }
    }
}
namespace AdventOfCode2023.Day6;

// https://adventofcode.com/2023/day/6

internal static class Part2
{
    public static void Run(string[] lines)
    {
        var timeEntries = new string(lines[0].Where(c => c != ' ').ToArray()).Split(':').ToList();
        var distanceEntries = new string(lines[1].Where(c => c != ' ').ToArray()).Split(':').ToList();

        var race = new Race
        {
            TotalTime = long.Parse(timeEntries[1]),
            DistanceRecord = long.Parse(distanceEntries[1])
        };
        
        for (var buttonHoldTime = 0; buttonHoldTime < race.TotalTime; buttonHoldTime++)
        {
            var speed = buttonHoldTime;
            var boatMovementTime = race.TotalTime - buttonHoldTime;
            var distance = boatMovementTime * speed;

            if (distance > race.DistanceRecord)
            {
                race.PossibleWaysToWin++;
            }
        }
        
        Console.WriteLine($"Total ways to win: {race.PossibleWaysToWin}");
    }
}
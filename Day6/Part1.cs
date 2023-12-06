namespace AdventOfCode2023.Day6;

// https://adventofcode.com/2023/day/6

internal static class Part1
{
    public static void Run(string[] lines)
    {
        var timeEntries = lines[0].Split(' ').Where(l => !String.IsNullOrWhiteSpace(l)).ToList();
        timeEntries.RemoveAt(0); // Remove title entry

        var distanceEntries = lines[1].Split(' ').Where(l => !String.IsNullOrWhiteSpace(l)).ToList();
        distanceEntries.RemoveAt(0); // Remove title entry
        
        var races = timeEntries.Select((t, i) => new Race
        {
            TotalTime = long.Parse(t),
            DistanceRecord = long.Parse(distanceEntries[i])
        }).ToList();
        
        long totalWaysToWin = 0;

        foreach (var race in races)
        {
            for (var buttonHoldTime = 0; buttonHoldTime < race.TotalTime; buttonHoldTime++)
            {
                var distance = 0;
                var speed = 0;
                
                for (var raceTime = 0; raceTime < race.TotalTime; raceTime++)
                {
                    var buttonHeld = buttonHoldTime > raceTime;

                    if (buttonHeld)
                    {
                        speed++;
                    }
                    else
                    {
                        distance += speed;
                    }
                }
                
                if (distance > race.DistanceRecord)
                {
                    race.PossibleWaysToWin++;
                }
            }
            
            Console.WriteLine($"Total ways to win race with {race.TotalTime} time and {race.DistanceRecord} distance record: {race.PossibleWaysToWin}");

            totalWaysToWin = totalWaysToWin == 0 ? race.PossibleWaysToWin : totalWaysToWin * race.PossibleWaysToWin;
        }
        
        Console.WriteLine($"Total ways to win: {totalWaysToWin}");
    }
}
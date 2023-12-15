namespace AdventOfCode2023.Day14;

// https://adventofcode.com/2023/day/14

internal static class Part2
{
    public static void Run(string[] lines)
    {
        var rolledRows = lines.ToList();
        var loadHistory = new List<long>();
        var loadPattern = new List<long>();
        var patternIndexBegin = 0;
        var maxCycles = 1000;

        for (var i = 0; i < maxCycles; i++)
        {
            rolledRows = RollRoundRocks(rolledRows, Direction.North);
            rolledRows = RollRoundRocks(rolledRows, Direction.West);
            rolledRows = RollRoundRocks(rolledRows, Direction.South);
            rolledRows = RollRoundRocks(rolledRows, Direction.East);
            
            var load = GetLoadOnNorthSupportBeams(rolledRows);

            if (i > 10)
            {
                var index = loadHistory.IndexOf(load);

                if (index > 2)
                {
                    if (loadHistory[i - 1] == loadHistory[index - 1] && loadHistory[i - 2] == loadHistory[index - 2] && loadHistory[i - 2] == loadHistory[index - 2])
                    {
                        loadPattern = loadHistory.GetRange(index, i - index);
                        patternIndexBegin = index;
                        break;
                    }
                }
            }
            
            loadHistory.Add(load);

            Console.WriteLine($"Load on north support beams after {i} cycles: {load}");
        }

        var desiredCycleCount = 1000000000 - 1;
        long finalLoad = 0;

        for (var i=patternIndexBegin; i < desiredCycleCount; i += loadPattern.Count)
        {
            if (i + loadPattern.Count > desiredCycleCount)
            {
                var diff = desiredCycleCount - i;
                finalLoad = loadPattern[diff];
            }
        }

        Console.WriteLine($"Final load on north support beams: {finalLoad}");
    }

    public static List<string> RollRoundRocks(List<string> rows, Direction direction)
    {
        var rolledEntries = new List<string>();
        var verticalDirection = direction == Direction.North || direction == Direction.South;
        var descendingOrder = direction == Direction.North || direction == Direction.West;
        var entryCount = verticalDirection ? rows.First().Length : rows.Count;
        
        // Roll the round rocks north by sorting columns or rows between cube rocks
        for (var i = 0; i < entryCount; i++)
        {
            // Can be either a row or column
            string entry = verticalDirection ? string.Join("", rows.Select(r => r[i])) : rows[i];
            
            var sections = entry.Split("#");
            var orderedSections = new List<string?>();

            foreach (var section in sections)
            {
                var orderedSection = descendingOrder ?
                    string.Join("", section.ToCharArray().OrderDescending())
                    : string.Join("", section.ToCharArray().Order());
                
                orderedSections.Add(orderedSection);
            }
            
            rolledEntries.Add(string.Join('#', orderedSections));
        }

        if (!verticalDirection)
        {
            return rolledEntries;
        }

        // Always return as list of rows, not columns
        var rolledRows = new List<string>();

        for (var i = 0; i < rolledEntries.First().Length; i++)
        {
            rolledRows.Add(string.Join("", rolledEntries.Select(e => e[i])));
        }
        
        return rolledRows;
    }

    public static long GetLoadOnNorthSupportBeams(List<string> rolledRows)
    {
        long loadOnNorthSupportBeams = 0;
        long loadMultiplier = rolledRows.Count;
        
        // Count load from round rocks in each row
        foreach (var row in rolledRows)
        {
            long roundedRockCount = row.Count(c => c == 'O');

            loadOnNorthSupportBeams += (roundedRockCount * loadMultiplier);
            loadMultiplier--;
        }

        return loadOnNorthSupportBeams;
    }
}

enum Direction
{
    North,
    East,
    South,
    West
}
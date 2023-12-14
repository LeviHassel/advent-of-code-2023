namespace AdventOfCode2023.Day14;

// https://adventofcode.com/2023/day/14

internal static class Part1
{
    public static void Run(string[] lines)
    {
        // Roll the round rocks north by sorting columns between cube rocks
        var rolledColumns = new List<string>();
        
        for (var i = 0; i < lines.First().Length; i++)
        {
            string column = string.Join("", lines.Select(r => r[i]));
            
            var sections = column.Split("#");
            var orderedSections = new List<string?>();

            foreach (var section in sections)
            {
                var orderedSection = string.Join("", section.ToCharArray().OrderDescending());
                orderedSections.Add(orderedSection);
            }

            rolledColumns.Add(string.Join('#', orderedSections));
        }

        var loadOnNorthSupportBeams = 0;
        var loadMultiplier = lines.Length;
        
        // Count load from round rocks in each row
        for (var i = 0; i < rolledColumns.First().Length; i++)
        {
            string rolledRow = string.Join("", rolledColumns.Select(r => r[i]));

            var roundedRockCount = rolledRow.Count(c => c == 'O');

            loadOnNorthSupportBeams += (roundedRockCount * loadMultiplier);
            loadMultiplier--;
        }

        Console.WriteLine($"Load on north support beams: {loadOnNorthSupportBeams}");
    }
}
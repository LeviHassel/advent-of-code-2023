namespace AdventOfCode2023.Day11;

// https://adventofcode.com/2023/day/11

internal static class Part1
{
    public static void Run(string[] lines)
    {
        var rows = lines.ToList();

        // Expand rows
        for (var i = 0; i < rows.Count(); i++)
        {
            var row = rows[i];
            
            if (row.All(character => character == '.'))
            {
                rows.Insert(i, row);
                i++;
            }
        }
        
        // Expand columns
        for (var i = 0; i < rows.First().Length; i++)
        {
            var column = rows.Select(r => r[i]).ToList();
            
            if (column.All(character => character == '.'))
            {
                for (var j = 0; j < rows.Count; j++)
                {
                    rows[j] = rows[j].Insert(i, ".");
                }
                
                i++;
            }
        }

        var galaxies = new List<Galaxy>();
        var galaxyId = 1;
        
        // Identify galaxies
        for (var i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            
            for (var j = 0; j < row.Length; j++)
            {
                var character = row[j];
                if (character == '#')
                {
                    galaxies.Add(new Galaxy()
                    {
                        Id = galaxyId,
                        ExpandedRow = i,
                        ExpandedColumn = j
                    });

                    galaxyId++;
                }
            }
        }
        
        // Create a list of all possible pairs of galaxies
        var galaxyPairs = galaxies.SelectMany(x => galaxies, Tuple.Create)
            .Where(tuple => tuple.Item1.Id < tuple.Item2.Id).ToList();

        long shortestPathTotal = 0;
        
        // Calculate shortest paths between galaxies
        foreach (var galaxyPair in galaxyPairs)
        {
            shortestPathTotal += Math.Abs(galaxyPair.Item1.ExpandedRow - galaxyPair.Item2.ExpandedRow) + Math.Abs(galaxyPair.Item1.ExpandedColumn - galaxyPair.Item2.ExpandedColumn);
        }
        
        Console.WriteLine($"Sum of shortest paths between galaxies: {shortestPathTotal}");
    }
}
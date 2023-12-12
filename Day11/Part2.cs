namespace AdventOfCode2023.Day11;

// https://adventofcode.com/2023/day/11

internal static class Part2
{
    public static void Run(string[] lines)
    {
        var rows = lines.ToList();
        
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
                        OriginalRow = i,
                        OriginalColumn = j,
                        ExpandedRow = i,
                        ExpandedColumn = j
                    });

                    galaxyId++;
                }
            }
        }

        // Account for row expansion in galaxy coordinates
        for (var i = 0; i < rows.Count(); i++)
        {
            var row = rows[i];
            
            if (row.All(character => character == '.'))
            {
                var galaxiesBeneathRow = galaxies.Where(g => g.OriginalRow > i);

                foreach (var galaxy in galaxiesBeneathRow)
                {
                    galaxy.ExpandedRow += (1000000 - 1);
                }
            }
        }
        
        // Account for column expansion in galaxy coordinates
        for (var i = 0; i < rows.First().Length; i++)
        {
            var column = rows.Select(r => r[i]).ToList();
            
            if (column.All(character => character == '.'))
            {
                var galaxiesToRightOfColumn = galaxies.Where(g => g.OriginalColumn > i);

                foreach (var galaxy in galaxiesToRightOfColumn)
                {
                    galaxy.ExpandedColumn += (1000000 - 1);
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
            var shortestPathBetwwenGalaxies = Math.Abs(galaxyPair.Item1.ExpandedRow - galaxyPair.Item2.ExpandedRow) + Math.Abs(galaxyPair.Item1.ExpandedColumn - galaxyPair.Item2.ExpandedColumn);
            shortestPathTotal += shortestPathBetwwenGalaxies;
        }
        
        Console.WriteLine($"Sum of shortest paths between galaxies: {shortestPathTotal}");
    }
}
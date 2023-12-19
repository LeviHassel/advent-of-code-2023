namespace AdventOfCode2023.Day18;

// https://adventofcode.com/2023/day/18

internal static class Part1
{
    public static void Run(string[] lines)
    {
        var trenchBorderTiles = new HashSet<(int, int)> { new(1, 1) };
        var currentLocation = new Tuple<int, int>(1, 1);

        // Dig trench
        foreach (var line in lines)
        {
            var contents = line.Split(' ');
            var direction = GetDirection(contents[0]);
            var meters = int.Parse(contents[1]);
            var color = contents[2].ToCharArray()[2..7];

            var digTrenchResponse = DigTrench(direction, meters, currentLocation.Item1, currentLocation.Item2);
            var trenchLine = digTrenchResponse.Item1;
            currentLocation = digTrenchResponse.Item2;
            trenchBorderTiles.UnionWith(trenchLine);
        }
        
        // Fill in tiles inside trench
        var filledTiles = GetTilesToFill(trenchBorderTiles);
        
        // Handle if the trench crosses over itself (like first and last entry)
        var distinctTrenchTiles = trenchBorderTiles.DistinctBy(t => new Tuple<int, int>(t.Item1, t.Item2)).ToList();

        var lavaTileCount = distinctTrenchTiles.Count + filledTiles.Count;
        
        PrintTrench(trenchBorderTiles, filledTiles);
        
        Console.WriteLine($"Cubic meters of lava trench can hold: {lavaTileCount}");
    }

    public static (HashSet<(int, int)>, Tuple<int, int>) DigTrench(Direction direction, int meters, int startingX, int startingY)
    {
        var trenchTiles = new HashSet<(int, int)>();

        var finalX = startingX;
        var finalY = startingY;

        var newX = startingX;
        var newY = startingY;
        
        switch (direction)
        {
            case Direction.Right:
                newX += meters;
                finalX = newX;
                for (var x = startingX + 1; x <= newX; x++)
                {
                    trenchTiles.Add((x, startingY));
                }
                break;
            case Direction.Down:
                newY += meters;
                finalY = newY;
                for (var y = startingY + 1; y <= newY; y++)
                {
                    trenchTiles.Add((startingX, y));
                }
                break;
            case Direction.Left:
                newX -= meters;
                finalX = newX;
                for (var x = startingX - 1; x >= newX; x--)
                {
                    trenchTiles.Add((x, startingY));
                }
                break;
            case Direction.Up:
                newY -= meters;
                finalY = newY;
                for (var y = startingY - 1; y >= newY; y--)
                {
                    trenchTiles.Add((startingX, y));
                }
                break;
        }

        return (trenchTiles, new Tuple<int, int>(finalX, finalY));
    }

    public static HashSet<(int, int)> GetTilesToFill(HashSet<(int, int)> trenchBorderTiles)
    {
        var filledTiles = new HashSet<(int, int)>();
        var tileQueue = new Queue<(int, int)>();

        var rowWithOnlyTwoTrenchTiles = trenchBorderTiles.GroupBy(t => t.Item2).First(g => g.Count() == 2);
        var xForFirstTileInRow = rowWithOnlyTwoTrenchTiles.Min(t => t.Item1);
        var xForSecondTileInRow = rowWithOnlyTwoTrenchTiles.Max(t => t.Item1);
        var xInsideTrench = xForFirstTileInRow + (xForSecondTileInRow - xForFirstTileInRow) / 2;

        tileQueue.Enqueue((xInsideTrench, rowWithOnlyTwoTrenchTiles.Key));

        while (tileQueue.Any())
        {
            var tile = tileQueue.Dequeue();
            
            Console.WriteLine($"Filled tiles: {filledTiles.Count}, Tile filling queue: {tileQueue.Count}, Current tile: ({tile.Item1}, {tile.Item2})");
            
            if (ShouldTileBeFilled(tile.Item1, tile.Item2, trenchBorderTiles, filledTiles))
            {
                filledTiles.Add(tile);
                
                var currentX = tile.Item1;
                var currentY = tile.Item2;
                var rightX = currentX + 1;
                var leftX = currentX - 1;
                var upY = currentY - 1;
                var downY = currentY + 1;

                // Enqueue left tile
                tileQueue.Enqueue((leftX, currentY));

                // Enqueue right tile
                tileQueue.Enqueue((rightX, currentY));

                // Enqueue up tile
                tileQueue.Enqueue((currentX, upY));

                // Enqueue down tile
                tileQueue.Enqueue((currentX, downY));
            }
        }

        return filledTiles;
    }
    
    public static bool ShouldTileBeFilled(int x, int y, HashSet<(int, int)> trenchBorderTiles, HashSet<(int, int)> filledTiles)
    {
        if (filledTiles.Contains((x, y)))
        {
            return false;
        }

        if (trenchBorderTiles.Contains((x, y)))
        {
            return false;
        }

        return true;
    }

    public static void PrintTrench(HashSet<(int, int)> trenchBorderTiles, HashSet<(int, int)> filledTiles)
    {
        var minY = trenchBorderTiles.Min(t => t.Item2);
        var maxY = trenchBorderTiles.Max(t => t.Item2);
        var minX = trenchBorderTiles.Min(t => t.Item1);
        var maxX = trenchBorderTiles.Max(t => t.Item1);
        
        for (var y = minY; y < maxY; y ++)
        {
            var line = "";

            for (var x = minX; x <= maxX; x++)
            {
                var isTrenchTile = trenchBorderTiles.Contains((x, y));
                var isFilledTile = filledTiles.Contains((x, y));
                
                line += isTrenchTile ? "# " : isFilledTile ? "o " : ". ";
            }
            
            Console.WriteLine(line);
        }
    }

    public static Direction GetDirection(string letter)
    {
        return letter switch
        {
            "R" => Direction.Right,
            "D" => Direction.Down,
            "L" => Direction.Left,
            "U" => Direction.Up,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
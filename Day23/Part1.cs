using System.Text;

namespace AdventOfCode2023.Day23;

// https://adventofcode.com/2023/day/23

internal static class Part1
{
    public static void Run(string[] lines)
    {
        var startingX = 1;
        var startingY = 1;
        var endingX = lines.First().Length - 2;
        var endingY = lines.Length - 1;
        
        var pathCount = 1;

        var tileQueue = new Queue<(int, int, int)>();

        var firstPath = new Path()
        {
            Id = pathCount++,
            Steps = new List<Tuple<int, int>>()
        };
        
        var paths = new List<Path> { firstPath };

        tileQueue.Enqueue((startingX, startingY, firstPath.Id));

        while (tileQueue.Any())
        {
            var (x, y, pathId) = tileQueue.Dequeue();

            var path = paths.Find(p => p.Id == pathId);
            
            path.Steps.Add( new Tuple<int, int>(x, y));

            if (x == endingX && y == endingY)
            {
                path.IsComplete = true;
                continue;
            }
            
            Console.WriteLine($"Path: {path.Id} ({path.Steps.Count} steps), Tile exploring queue: {tileQueue.Count}, Current tile: ({x}, {y})");

            var tilesToExplore = GetTilesToExplore(x, y, lines);
            var newStepAddedToCurrentPath = false;

            foreach (var tile in tilesToExplore)
            {
                var tileX = tile.Item1;
                var tileY = tile.Item2;
                
                if (CanExploreTile(tileX, tileY, lines) && !path.Steps.Contains(new Tuple<int, int>(tileX, tileY)))
                {
                    var messagePathId = pathId;

                    if (newStepAddedToCurrentPath)
                    {
                        var newPath = new Path
                        {
                            Id = pathCount++,
                            Steps = path.Steps.Select(p => new Tuple<int, int>(p.Item1, p.Item2)).ToList()
                        };
                    
                        paths.Add(newPath);
                        messagePathId = newPath.Id;
                    }
                    else
                    {
                        newStepAddedToCurrentPath = true;
                    }

                    tileQueue.Enqueue((tileX, tileY, messagePathId));
                }
            }
        }

        var longestPath = paths.Where(p => p.IsComplete).OrderByDescending(p => p.Steps.Count).First();
        
        PrintMap(longestPath, lines);
        
        Console.WriteLine($"The longest hike you can take is {longestPath.Steps.Count} steps");
    }

    public static bool CanExploreTile(int x, int y, string[] lines)
    {
        if (y < 0 || y > lines.Length - 1)
        {
            return false;
        }
        
        if (x < 0 || x > lines.First().Length - 1)
        {
            return false;
        }

        if (lines[y][x] == '#')
        {
            return false;
        }

        return true;
    }

    public static List<Tuple<int, int>> GetTilesToExplore(int x, int y, string[] lines)
    {
        var tilesToExplore = new List<Tuple<int, int>>();

        var character = lines[y][x];

        switch (character)
        {
            case '.':
                tilesToExplore.Add(new Tuple<int, int>(x, y - 1));
                tilesToExplore.Add(new Tuple<int, int>(x + 1, y));
                tilesToExplore.Add(new Tuple<int, int>(x, y + 1));
                tilesToExplore.Add(new Tuple<int, int>(x - 1, y));
                break;
            case '^':
                tilesToExplore.Add(new Tuple<int, int>(x, y - 1));
                break;
            case '>':
                tilesToExplore.Add(new Tuple<int, int>(x + 1, y));
                break;
            case 'v':
                tilesToExplore.Add(new Tuple<int, int>(x, y + 1));
                break;
            case '<':
                tilesToExplore.Add(new Tuple<int, int>(x - 1, y));
                break;
        }

        return tilesToExplore;
    }
    
    public static void PrintMap(Path path, string[] lines)
    {
        var maxY = lines.Length - 1;
        var maxX = lines.First().Length - 1;
        
        for (var y = 0; y <= maxY; y ++)
        {
            var line = lines[y];

            for (var x = 0; x <= maxX; x++)
            {
                if (path.Steps.Any(s => s.Item1 == x && s.Item2 == y))
                {
                    var updatedLine = new StringBuilder(line);
                    updatedLine[x] = 'O';
                    line = updatedLine.ToString();
                }
            }
            
            Console.WriteLine(line);
        }
    }
}

public class Path
{
    public int Id;
    public List<Tuple<int, int>> Steps;
    public bool IsComplete;
}
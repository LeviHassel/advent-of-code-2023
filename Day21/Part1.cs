namespace AdventOfCode2023.Day21;

// https://adventofcode.com/2023/day/21

internal static class Part1
{
    public static void Run(string[] lines)
    {
        var stepsToTake = 64;
        
        var startingY = lines.ToList().FindIndex(l => l.Contains('S'));
        var startingX = lines[startingY].IndexOf('S');
        
        var plotQueue = new Queue<(int, int, int)>();
        var plotsAtEnd = new HashSet<(int, int)>();
        var exploredTilesForStep = new HashSet<(int, int, int)>();
        
        plotQueue.Enqueue((startingX, startingY, 0));

        while (plotQueue.Any())
        {
            var plot = plotQueue.Dequeue();
            exploredTilesForStep.Add((plot.Item1, plot.Item2, plot.Item3));
            
            Console.WriteLine($"Steps: {plot.Item3}, Plot exploring queue: {plotQueue.Count}, Current plot: ({plot.Item1}, {plot.Item2})");
            
            if (ShouldPlotBeExplored(plot.Item1, plot.Item2, lines))
            {
                var currentX = plot.Item1;
                var currentY = plot.Item2;
                var newStepCount = plot.Item3 + 1;
                var rightX = currentX + 1;
                var leftX = currentX - 1;
                var upY = currentY - 1;
                var downY = currentY + 1;

                if (newStepCount <= stepsToTake)
                {
                    // Enqueue left plot
                    if (!plotQueue.Contains((leftX, currentY, newStepCount)) && !exploredTilesForStep.Contains((leftX, currentY, newStepCount)))
                    {
                        plotQueue.Enqueue((leftX, currentY, newStepCount));
                    }

                    // Enqueue right plot
                    if (!plotQueue.Contains((rightX, currentY, newStepCount)) && !exploredTilesForStep.Contains((rightX, currentY, newStepCount)))
                    {
                        plotQueue.Enqueue((rightX, currentY, newStepCount));
                    }

                    // Enqueue up plot
                    if (!plotQueue.Contains((currentX, upY, newStepCount)) && !exploredTilesForStep.Contains((currentX, upY, newStepCount)))
                    {
                        plotQueue.Enqueue((currentX, upY, newStepCount));
                    }

                    // Enqueue down plot
                    if (!plotQueue.Contains((currentX, downY, newStepCount)) && !exploredTilesForStep.Contains((currentX, downY, newStepCount)))
                    {
                        plotQueue.Enqueue((currentX, downY, newStepCount));
                    }
                }
                else
                {
                    if (!plotsAtEnd.Contains((currentX, currentY)))
                    {
                        plotsAtEnd.Add((currentX, currentY));
                    }
                }
            }
        }
        
        Console.WriteLine($"Garden plots that the elf could reach in {stepsToTake} steps: {plotsAtEnd.Count}");
    }

    public static bool ShouldPlotBeExplored(int x, int y, string[] lines)
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
}
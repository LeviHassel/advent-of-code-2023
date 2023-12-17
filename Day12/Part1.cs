namespace AdventOfCode2023.Day12;

// https://adventofcode.com/2023/day/12

internal static class Part1
{
    public static void Run(string[] lines)
    {
        var totalPossibleArrangementCount = 0;

        foreach (var line in lines)
        {
            var contents = line.Split(' ');
            var springs = contents[0];
            var damagedSpringGroupSizes = contents[1].Split(',').Select(s => int.Parse(s)).ToList(); // size of each contiguous group of damaged springs

            var operationalSpringRanges = GetOperationalSpringSizeRanges(springs, damagedSpringGroupSizes);
            var possibleOperationalSpringSizes = GetPossibleOperationalSpringSizes(new List<int>(), operationalSpringRanges);
            var totalOperationalSprings = operationalSpringRanges.First().MaxSize + operationalSpringRanges.GetRange(1, operationalSpringRanges.Count - 1).Sum(r => r.MinSize);
            possibleOperationalSpringSizes = possibleOperationalSpringSizes.Where(s => s.Sum() == totalOperationalSprings).ToList();
            
            var possibleArrangements = GetPossibleArrangements(springs, damagedSpringGroupSizes, possibleOperationalSpringSizes);
            totalPossibleArrangementCount += possibleArrangements.Count;
        }
        
        Console.WriteLine($"Total possible number of arrangements: {totalPossibleArrangementCount}");
    }

    public static List<OperationalSpringSizeRange> GetOperationalSpringSizeRanges(string line, List<int> damagedSpringGroupSizes)
    {
        var operationalSpringRanges = new List<OperationalSpringSizeRange>();
        var totalSprings = line.Length;
        var totalDamagedSprings = damagedSpringGroupSizes.Sum();
        var totalOperationalSprings = totalSprings - totalDamagedSprings;
        var operationalSpringGroupCount = damagedSpringGroupSizes.Count + 1;
        
        for (var i = 0; i < operationalSpringGroupCount; i++)
        {
            operationalSpringRanges.Add(new OperationalSpringSizeRange
            {
                MinSize = i > 0 && i < operationalSpringGroupCount - 1 ? 1 : 0
            });
        }

        var totalMinSizeOfRanges = operationalSpringRanges.Sum(r => r.MinSize);

        foreach (var operationalSpringRange in operationalSpringRanges)
        {
            operationalSpringRange.MaxSize = operationalSpringRange.MinSize + (totalOperationalSprings - totalMinSizeOfRanges);
        }

        return operationalSpringRanges;
    }
    
    public static List<List<int>> GetPossibleOperationalSpringSizes(List<int> startingOperationalSpringSizes, List<OperationalSpringSizeRange> operationalSpringRanges)
    {
        var possibleOperationalSpringSizes = new List<List<int>>();

        var firstOperationalSpringRange = operationalSpringRanges.First();

        for (var i=firstOperationalSpringRange.MinSize; i <= firstOperationalSpringRange.MaxSize; i++)
        {
            var nextOperationalSpringRanges = new List<OperationalSpringSizeRange>();
            
            for (var j=1; j < operationalSpringRanges.Count; j++)
            {
                var operationalSpringRange = operationalSpringRanges[j];
                nextOperationalSpringRanges.Add(new OperationalSpringSizeRange
                {
                    MinSize = operationalSpringRange.MinSize,
                    MaxSize = operationalSpringRange.MaxSize - (i - firstOperationalSpringRange.MinSize)
                });
            }
            if (nextOperationalSpringRanges.Count > 0)
            {
                var nextStartingOperationalSpringSizes = startingOperationalSpringSizes.Select(s => s).ToList();
                nextStartingOperationalSpringSizes.Add(i);
                var sizes = GetPossibleOperationalSpringSizes(nextStartingOperationalSpringSizes, nextOperationalSpringRanges);
                possibleOperationalSpringSizes.AddRange(sizes);
            }
            else
            {
                var finalOperationalSpringSizes = startingOperationalSpringSizes.Select(s => s).ToList();
                finalOperationalSpringSizes.Add(i);
                possibleOperationalSpringSizes.Add(finalOperationalSpringSizes);
            }
        }

        return possibleOperationalSpringSizes;
    }

    public static List<string> GetPossibleArrangements(string line, List<int> damagedSpringGroupSizes, List<List<int>> possibleOperationalSpringSizes)
    {
        var potentialLines = new List<string>();
        
        foreach (var operationalSpringSizes in possibleOperationalSpringSizes)
        {
            var potentialLine = "";
            
            for (var i=0; i < damagedSpringGroupSizes.Count; i++)
            {
                var operationalSpringSize = operationalSpringSizes[i];
                
                for (var j=0; j < operationalSpringSize; j++)
                {
                    potentialLine += '.';
                }
                
                var damagedSpringGroupSize = damagedSpringGroupSizes[i];
                
                for (var j=0; j < damagedSpringGroupSize; j++)
                {
                    potentialLine += '#';
                }
            }
            
            var lastOperationalSpringSize = operationalSpringSizes.Last();
                
            for (var i=0; i < lastOperationalSpringSize; i++)
            {
                potentialLine += '.';
            }
            
            potentialLines.Add(potentialLine);
        }
        
        var possibleLines = new List<string>();
        var knownOperationalSpringIndexes = Enumerable.Range(0, line.Length).Where(i => line[i] == '.').ToList();
        var knownDamagedSpringIndexes = Enumerable.Range(0, line.Length).Where(i => line[i] == '#').ToList();

        foreach (var potentialLine in potentialLines)
        {
            var lineValid = true;
            
            foreach (var knownOperationalSpringIndex in knownOperationalSpringIndexes)
            {
                if (potentialLine[knownOperationalSpringIndex] != '.')
                {
                    lineValid = false;
                    break;
                }
            }

            if (!lineValid)
            {
                continue;
            }
            
            foreach (var knownDamagedSpringIndex in knownDamagedSpringIndexes)
            {
                if (potentialLine[knownDamagedSpringIndex] != '#')
                {
                    lineValid = false;
                    break;
                }
            }
            
            if (!lineValid)
            {
                continue;
            }
            
            possibleLines.Add(potentialLine);
        }

        return possibleLines;
    }
}
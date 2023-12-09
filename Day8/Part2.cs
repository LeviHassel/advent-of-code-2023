namespace AdventOfCode2023.Day8;

// https://adventofcode.com/2023/day/8

internal static class Part2
{
    public static void Run(string[] lines)
    {
        var directions = lines.First().ToCharArray(); // can be either "R" or "L"

        var nodeLines = lines.ToList();
        nodeLines.RemoveRange(0, 2); // Remove directions line and blank line

        var nodes = new List<Node>();

        foreach (var nodeLine in nodeLines)
        {
            var contents = nodeLine.Split(" = ");
            var source = contents[0];
            var destinations = contents[1].Split(", ");
            var leftDestination = destinations[0].Remove(0, 1);
            var rightDestination = destinations[1].Remove(3, 1);
            
            nodes.Add(new Node()
            {
                Source = source,
                LeftDestination = leftDestination,
                RightDestination = rightDestination
            });
        }
        
        // Start with nodes that end with A
        var paths = nodes.Where(n => n.Source[2] == 'A').Select(n => n.Source).Select(n =>
            new Path
            {
                FirstNode = n,
                CurrentNode = n
            }).ToList();

        var steps = 1;
        
        while (paths.Any(p => p.StepsBetweenZNodes == 0))
        {
            foreach (var direction in directions)
            {
                foreach (var path in paths)
                {
                    if (path.StepsBetweenZNodes != 0)
                    {
                        continue;
                    }
                    
                    var node = nodes.First(n => n.Source == path.CurrentNode);

                    if (direction == 'L')
                    {
                        path.CurrentNode = node.LeftDestination;
                    }
                    else if (direction == 'R')
                    {
                        path.CurrentNode = node.RightDestination;
                    }

                    if (path.CurrentNode[2] == 'Z')
                    {
                        Console.WriteLine($"Path starting with {path.FirstNode} is now at {path.CurrentNode} after {steps} steps");

                        if (path.StepsToFirstZNode == 0)
                        {
                            path.FirstZNode = path.CurrentNode;
                            path.StepsToFirstZNode = steps;
                        }
                        else if (path.CurrentNode == path.FirstZNode)
                        {
                            path.StepsBetweenZNodes = steps - path.StepsToFirstZNode;
                        }
                    }
                }

                steps++;
            }
        }
        
        foreach (var path in paths)
        {
            Console.WriteLine($"Path's first node: {path.FirstNode}, first Z node: {path.FirstZNode}, steps to first Z node: {path.StepsToFirstZNode}, steps between Z nodes: {path.StepsBetweenZNodes}");
        }

        long stepsBetweenZNodesForFirstPath = paths.First().StepsBetweenZNodes;
        long totalStepsRequired = stepsBetweenZNodesForFirstPath;
        
        while (paths.Any(p => totalStepsRequired % p.StepsBetweenZNodes != 0))
        {
            totalStepsRequired += stepsBetweenZNodesForFirstPath;
        }

        Console.WriteLine($"Steps required: {totalStepsRequired}");
    }

    class Path
    {
        public string FirstNode;
        public string CurrentNode;
        public string FirstZNode;
        public long StepsToFirstZNode;
        public long StepsBetweenZNodes;
    }
}
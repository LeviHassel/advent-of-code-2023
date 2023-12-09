namespace AdventOfCode2023.Day8;

// https://adventofcode.com/2023/day/8

internal static class Part1
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

        var stepsRequired = 0;
        var currentNode = "AAA";

        while (currentNode != "ZZZ")
        {
            foreach (var direction in directions)
            {
                var node = nodes.First(n => n.Source == currentNode);

                if (direction == 'L')
                {
                    currentNode = node.LeftDestination;
                }
                else if (direction == 'R')
                {
                    currentNode = node.RightDestination;
                }
            
                stepsRequired++;

                if (currentNode == "ZZZ")
                {
                    break;
                }
            }
        }

        Console.WriteLine($"Steps required: {stepsRequired}");
    }
}
namespace AdventOfCode2023.Day17;

// https://adventofcode.com/2023/day/17

internal static class Part1
{
    public static void Run(string[] lines)
    {
        var unvisitedBlocks = GetBlocks(lines);
        var visitedBlocks = new HashSet<Block>();
        var currentBlock = unvisitedBlocks.First(b => b.X == 0 && b.Y == 0);
        var maxX = unvisitedBlocks.Max(b => b.X);
        var maxY = unvisitedBlocks.Max(b => b.Y);
        var visitableBlockCount = 1;

        while (visitableBlockCount > 0)
        {
            FindHeatLossForSurroundingBlocks(currentBlock, unvisitedBlocks, maxX, maxY);

            visitedBlocks.Add(currentBlock);
            unvisitedBlocks.Remove(currentBlock);
            
            var visitableBlocks = unvisitedBlocks.Where(b => b.MinHeatLossToBlock > -1).ToHashSet();
            visitableBlockCount = visitableBlocks.Count;

            if (visitableBlockCount == 0)
            {
                break;
            }
            
            currentBlock = visitableBlocks.Aggregate((curMin, b) => b.MinHeatLossToBlock < curMin.MinHeatLossToBlock
                ? b
                : curMin);
            
            Console.WriteLine($"Visited blocks: {visitedBlocks.Count}, Unvisited blocks: {unvisitedBlocks.Count}, Visitable blocks: {visitableBlocks.Count}");
        }

        var minHeatLossToDestinationBlock = visitedBlocks.Where(b => b.X == maxX && b.Y == maxY).Min(b => b.MinHeatLossToBlock);

        //PrintBlockPath(visitedBlocks);
        
        Console.WriteLine($"Minimum heat loss to destination block: {minHeatLossToDestinationBlock}");
    }

    public static HashSet<Block> GetBlocks(string[] lines)
    {
        var blocks = new HashSet<Block>();
        var directions = new List<Direction> { Direction.Right, Direction.Down, Direction.Left, Direction.Up };

        for (var i=0; i < lines.Length; i++)
        {
            var line = lines[i];
            
            for (var j=0; j < line.Length; j++)
            {
                if (i == 0 & j == 0)
                {
                    blocks.Add(new Block
                    {
                        X = j,
                        Y = i,
                        HeatLoss = 0,
                        MinHeatLossToBlock = 0,
                        ConsecutiveStepsInDirection = 0,
                        Direction = Direction.Start,
                    });
                    continue;
                }
                
                var heatLoss = int.Parse(line[j].ToString());
                
                foreach (var direction in directions)
                {
                    for (var k=0; k <= 3; k++)
                    {
                        blocks.Add(new Block
                        {
                            X = j,
                            Y = i,
                            Direction = direction,
                            ConsecutiveStepsInDirection = k,
                            HeatLoss = heatLoss,
                            MinHeatLossToBlock = -1,
                        });
                    }
                }
            }
        }

        return blocks;
    }

    public static void FindHeatLossForSurroundingBlocks(Block currentBlock, HashSet<Block> blocks, int maxX, int maxY)
    {
        var curX = currentBlock.X;
        var curY = currentBlock.Y;
        var rightX = curX + 1;
        var leftX = curX - 1;
        var upY = curY - 1;
        var downY = curY + 1;
        var consecutiveStraightMoves = currentBlock.ConsecutiveStepsInDirection;
        var lastMoveDirection = currentBlock.Direction;
        var directionChangeRequired = consecutiveStraightMoves >= 3;
        
        if (leftX >= 0 && lastMoveDirection != Direction.Right && !(directionChangeRequired && lastMoveDirection == Direction.Left))
        {
            UpdateBlockWithMinHeatLoss(currentBlock, blocks, Direction.Left, leftX, curY);
        }
        
        if (rightX <= maxX && lastMoveDirection != Direction.Left && !(directionChangeRequired && lastMoveDirection == Direction.Right))
        {
            UpdateBlockWithMinHeatLoss(currentBlock, blocks, Direction.Right, rightX, curY);
        }
        
        if (upY >= 0 && lastMoveDirection != Direction.Down && !(directionChangeRequired && lastMoveDirection == Direction.Up))
        {
            UpdateBlockWithMinHeatLoss(currentBlock, blocks, Direction.Up, curX, upY);
        }
        
        if (downY <= maxY && lastMoveDirection != Direction.Up && !(directionChangeRequired && lastMoveDirection == Direction.Down))
        {
            UpdateBlockWithMinHeatLoss(currentBlock, blocks, Direction.Down, curX, downY);
        }
    }

    public static void UpdateBlockWithMinHeatLoss(Block sourceBlock, HashSet<Block> blocks, Direction direction, int x, int y)
    {
        var consecutiveStepsInDirection = direction == sourceBlock.Direction ? sourceBlock.ConsecutiveStepsInDirection + 1 : 1; 
        var destBlock = blocks.FirstOrDefault(b => b.X == x && b.Y == y && b.Direction == direction && b.ConsecutiveStepsInDirection == consecutiveStepsInDirection);

        if (destBlock == null) return;
        var heatLossToBlock = sourceBlock.MinHeatLossToBlock + destBlock.HeatLoss;

        if (destBlock.MinHeatLossToBlock == -1 || heatLossToBlock < destBlock.MinHeatLossToBlock)
        {
            destBlock.MinHeatLossToBlock = heatLossToBlock;
            destBlock.Direction = direction;
            destBlock.ConsecutiveStepsInDirection = direction == sourceBlock.Direction ? sourceBlock.ConsecutiveStepsInDirection + 1 : 1;
        }
    }

    public static void PrintBlockPath(HashSet<Block> blocks)
    {
        var groupedBlocks = blocks.GroupBy(b => b.Y);

        foreach (var blockGroup in groupedBlocks)
        {
            var orderedBlockGroup = blockGroup.OrderBy(b => b.X);
            Console.WriteLine(string.Join("\t", orderedBlockGroup.Select(b => $"{b.MinHeatLossToBlock}{GetArrowStringForDirection(b.Direction)}({b.HeatLoss})")));
        }
    }
    
    public static char GetArrowStringForDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Down:
                return 'v';
            case Direction.Right:
                return '>';
            case Direction.Up:
                return '^';
            case Direction.Left:
                return '<';
            case Direction.Start:
                return 'o';
        }

        return ' ';
    }
}
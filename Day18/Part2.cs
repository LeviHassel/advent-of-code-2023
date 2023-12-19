namespace AdventOfCode2023.Day18;

// https://adventofcode.com/2023/day/18

internal static class Part2
{
    public static void Run(string[] lines)
    {
        var trenchLines = new List<TrenchLine>();
        var currentLocation = new Tuple<long, long>(1, 1);

        // Dig trench
        foreach (var line in lines)
        {
            var contents = line.Split(' ');
            var color = contents[2].ToCharArray()[2..8];
            var metersInHex = new string(color[..5]);
            var directionInHex = color[5].ToString();
            var direction = GetDirection(directionInHex);
            var meters = Convert.ToInt32(metersInHex, 16);

            var trenchLine = DigTrench(direction, meters, currentLocation.Item1, currentLocation.Item2);
            currentLocation = GetFinalLocation(trenchLine);
            
            trenchLines.Add(trenchLine);
        }
        
        // Fill in tiles inside trench
        var cubicMetersInsideBorders = GetCubicMetersInsideBorders(trenchLines);
        var cubicMetersOfBorders = trenchLines.Sum(l => l.Max - l.Min);

        var fillableCubicMeters = cubicMetersInsideBorders + cubicMetersOfBorders;
        
        Console.WriteLine($"Cubic meters of lava that trench can hold: {fillableCubicMeters}");
    }

    public static TrenchLine DigTrench(Direction direction, int meters, long startingX, long startingY)
    {
        return direction switch
        {
            Direction.Right => new TrenchLine() { Type = TrenchLineType.Horizontal, Min = startingX, Max = startingX + meters, ConstantDimension = startingY, Direction = Direction.Right},
            Direction.Down => new TrenchLine() { Type = TrenchLineType.Vertical, Min = startingY, Max = startingY + meters, ConstantDimension = startingX, Direction = Direction.Down},
            Direction.Left => new TrenchLine() { Type = TrenchLineType.Horizontal, Max = startingX, Min = startingX - meters, ConstantDimension = startingY, Direction = Direction.Left},
            Direction.Up => new TrenchLine() { Type = TrenchLineType.Vertical, Max = startingY, Min = startingY - meters, ConstantDimension = startingX, Direction = Direction.Up},
            _ => throw (new Exception("Could not handle direction correctly"))
        };
    }

    public static Tuple<long, long> GetFinalLocation(TrenchLine trenchLine)
    {
        return trenchLine.Direction switch
        {
            Direction.Right => new Tuple<long, long>(trenchLine.Max, trenchLine.ConstantDimension),
            Direction.Down => new Tuple<long, long>(trenchLine.ConstantDimension, trenchLine.Max),
            Direction.Left => new Tuple<long, long>(trenchLine.Min, trenchLine.ConstantDimension),
            Direction.Up => new Tuple<long, long>(trenchLine.ConstantDimension, trenchLine.Min),
            _ => throw (new Exception("Could not handle trench line correctly"))
        };
    }

    public static long GetCubicMetersInsideBorders(List<TrenchLine> trenchLines)
    {
        long cubicMeters = 0;

        var verticalTrenchLines = trenchLines.Where(l => l.Type == TrenchLineType.Vertical).OrderBy(l => l.ConstantDimension).ToList();
        var horizontalTrenchLines = trenchLines.Where(l => l.Type == TrenchLineType.Horizontal).OrderBy(l => l.Min).ToList();

        var minY = Math.Min(verticalTrenchLines.Min(l => l.Min), horizontalTrenchLines.Min(l => l.ConstantDimension));
        var maxY = Math.Min(verticalTrenchLines.Max(l => l.Max), horizontalTrenchLines.Max(l => l.ConstantDimension));
        
        for (long y = minY; y < maxY; y++)
        {
            var horizontalLines = horizontalTrenchLines.Where(l => l.ConstantDimension == y).ToList();
            var crossingVerticalLines = verticalTrenchLines.Where(l => l.Min <= y && l.Max >= y).ToList();

            var filledLinesForLog = new List<Tuple<long, long>>();

            var insideBorder = false;
            TrenchLine? lastLine = null;
            var lastLineTouchingHorizontalEdge = false;
            var borderCrossCount = 0;

            foreach (var verticalLine in crossingVerticalLines)
            {
                var lineX = verticalLine.ConstantDimension;
                
                var touchingHorizontalEdge = horizontalLines.Any(l => l.Min == lineX || l.Max == lineX);

                if (touchingHorizontalEdge)
                {
                    if (lastLineTouchingHorizontalEdge)
                    {
                        var leftLineGoesDown = lastLine.Max > y;
                        var rightLineGoesDown = verticalLine.Max > y;

                        insideBorder = borderCrossCount % 2 == 1
                            ? leftLineGoesDown == rightLineGoesDown
                            : leftLineGoesDown != rightLineGoesDown;
                        
                        lastLineTouchingHorizontalEdge = false;
                        lastLine = verticalLine;

                        if (leftLineGoesDown != rightLineGoesDown)
                        {
                            borderCrossCount++;
                        }
                        
                        continue;
                    }
                    
                    lastLineTouchingHorizontalEdge = true;
                }
                else
                {
                    lastLineTouchingHorizontalEdge = false;
                    borderCrossCount++;
                }
                
                if (insideBorder)
                {
                    filledLinesForLog.Add( new Tuple<long, long>(lastLine.ConstantDimension, lineX));
                    cubicMeters += lineX - lastLine.ConstantDimension - 1;
                    insideBorder = false;
                    lastLine = verticalLine;
                    
                    continue;
                }

                insideBorder = true;
                lastLine = verticalLine;
            }

            if (y % 1000000 == 0)
            {
                Console.WriteLine($"Cubic meters inside borders of trench by Y={y}/{maxY}: {cubicMeters}");
            }
            
            /*
            Uncomment to print rows
             
            var line = "";

            for (var x = minX; x <= maxX; x++)
            {
                if (horizontalLines.Any(l => l.Min <= x && l.Max >= x) || verticalEdges.Any(e => e.X == x))
                {
                    line += "# ";
                    continue;
                }
                
                if (filledLinesForLog.Any(l => l.Item1 <= x && l.Item2 >= x))
                {
                    line += "o ";
                    continue;
                }

                line += ". ";
            }
            
            Console.WriteLine(line);
            */
        }

        return cubicMeters;
    }

    public static Direction GetDirection(string letter)
    {
        return letter switch
        {
            "0" => Direction.Right,
            "1" => Direction.Down,
            "2" => Direction.Left,
            "3" => Direction.Up,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

internal class TrenchLine
{
    public long Min;
    public long Max;
    public long ConstantDimension;
    public TrenchLineType Type;
    public Direction Direction;
}

internal enum TrenchLineType
{
    Horizontal,
    Vertical
}
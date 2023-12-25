namespace AdventOfCode2023.Day24;

// https://adventofcode.com/2023/day/24

internal static class Part1
{
    private const double TestAreaStartingX = 200000000000000; // 7
    private const double TestAreaStartingY = 200000000000000; // 7
    private const double TestAreaEndingX = 400000000000000; // 27
    private const double TestAreaEndingY = 400000000000000; // 27
    
    public static void Run(string[] lines)
    {
        var hailstorm = GetHailstorm(lines);

        FindHailInTestArea(hailstorm);

        var totalIntersections = GetIntersectionsInTestArea(hailstorm);
        
        Console.WriteLine($"Total number of hail intersections: {totalIntersections}");
    }

    public static List<Hail> GetHailstorm(string[] lines)
    {
        var hailstorm = new List<Hail>();
        var hailId = 1;

        foreach (var line in lines)
        {
            var contents = line.Split(" @ ");
            var coordinates = contents[0].Split(", ");
            var velocity = contents[1].Split(", ");

            var hail = new Hail
            {
                Id = hailId++,
                VelocityX = int.Parse(velocity[0]),
                VelocityY = int.Parse(velocity[1])
            };

            var hailAtZeroNanoseconds = new Position()
            {
                X = long.Parse(coordinates[0]),
                Y = long.Parse(coordinates[1])
            };
            
            hail.PositionByNanosecond.Add(0, hailAtZeroNanoseconds);
            
            hail.Slope = hail.VelocityY / hail.VelocityX;
            hail.YIntercept = hailAtZeroNanoseconds.Y - hail.Slope * hailAtZeroNanoseconds.X;
            
            hailstorm.Add(hail);
        }

        return hailstorm;
    }
    
    public static void FindHailInTestArea(List<Hail> hailstorm)
    {
        foreach (var hail in hailstorm)
        {
            var startingPosition = hail.PositionByNanosecond.First().Value;

            // Add position that intersects with the starting X axis of the test area
            var nanosecondsToTestAreaStartX = (TestAreaStartingX - startingPosition.X) / hail.VelocityX;
            var yAtTestAreaStartX = startingPosition.Y + (nanosecondsToTestAreaStartX * hail.VelocityY);
            
            hail.PositionByNanosecond.Add(nanosecondsToTestAreaStartX, new Position()
            {
                X = TestAreaStartingX,
                Y = yAtTestAreaStartX
            });
            
            // Add position that intersects with the starting Y axis of the test area
            var nanosecondsToTestAreaStartY = (TestAreaStartingY - startingPosition.Y) / hail.VelocityY;
            var xAtTestAreaStartY = startingPosition.X + (nanosecondsToTestAreaStartY * hail.VelocityX);
            
            hail.PositionByNanosecond.Add(nanosecondsToTestAreaStartY, new Position()
            {
                X = xAtTestAreaStartY,
                Y = TestAreaStartingY
            });
            
            // Add position that intersects with the ending X axis of the test area
            var nanosecondsToTestAreaEndX = (TestAreaEndingX - startingPosition.X) / hail.VelocityX;
            var yAtTestAreaEndX = startingPosition.Y + (nanosecondsToTestAreaEndX * hail.VelocityY);
            
            hail.PositionByNanosecond.Add(nanosecondsToTestAreaEndX, new Position()
            {
                X = TestAreaEndingX,
                Y = yAtTestAreaEndX
            });
            
            // Add position that intersects with the ending Y axis of the test area
            var nanosecondsToTestAreaEndY = (TestAreaEndingY - startingPosition.Y) / hail.VelocityY;
            var xAtTestAreaEndY = startingPosition.X + (nanosecondsToTestAreaEndY * hail.VelocityX);
            
            hail.PositionByNanosecond.Add(nanosecondsToTestAreaEndY, new Position()
            {
                X = xAtTestAreaEndY,
                Y = TestAreaEndingY
            });
        }
    }

    public static int GetIntersectionsInTestArea(List<Hail> hailstorm)
    {
        var totalIntersections = 0;

        for (var i=0; i < hailstorm.Count; i++)
        {
            var mainHail = hailstorm[i];
            var mainHailYAtTestAreaStartX = mainHail.PositionByNanosecond.First(p => p.Value.X == TestAreaStartingX).Value.Y;
            var mainHailYAtTestAreaEndX = mainHail.PositionByNanosecond.First(p => p.Value.X == TestAreaEndingX).Value.Y;
            var mainHailXAtTestAreaStartY = mainHail.PositionByNanosecond.First(p => p.Value.Y == TestAreaStartingY).Value.X;
            var mainHailXAtTestAreaEndY = mainHail.PositionByNanosecond.First(p => p.Value.Y == TestAreaEndingY).Value.X;

            for (var j=  i + 1; j < hailstorm.Count; j++)
            {
                var hailToCompare = hailstorm[j];
                
                var hailToCompareYAtTestAreaStartX = hailToCompare.PositionByNanosecond.First(p => p.Value.X == TestAreaStartingX).Value.Y;
                var hailToCompareYAtTestAreaEndX = hailToCompare.PositionByNanosecond.First(p => p.Value.X == TestAreaEndingX).Value.Y;
                var hailToCompareXAtTestAreaStartY = hailToCompare.PositionByNanosecond.First(p => p.Value.Y == TestAreaStartingY).Value.X;
                var hailToCompareXAtTestAreaEndY = hailToCompare.PositionByNanosecond.First(p => p.Value.Y == TestAreaEndingY).Value.X;

                var crossesX = mainHailXAtTestAreaStartY > hailToCompareXAtTestAreaStartY != mainHailXAtTestAreaEndY > hailToCompareXAtTestAreaEndY;
                var crossesY = mainHailYAtTestAreaStartX > hailToCompareYAtTestAreaStartX != mainHailYAtTestAreaEndX > hailToCompareYAtTestAreaEndX;
                
                if (crossesX && crossesY && IsIntersectionValid(mainHail, hailToCompare))
                {
                    totalIntersections++;
                }
            }
        }

        return totalIntersections;
    }

    public static bool IsIntersectionValid(Hail mainHail, Hail hailToCompare)
    {
        double xIntersectionPosition = (hailToCompare.YIntercept - mainHail.YIntercept) / (mainHail.Slope - hailToCompare.Slope);
        double yIntersectionPosition = (mainHail.Slope * xIntersectionPosition) + mainHail.YIntercept;
        
        var mainHailAtZeroNanoseconds = mainHail.PositionByNanosecond.First(p => p.Key == 0).Value;
        var hailToCompareAtZeroNanoseconds = hailToCompare.PositionByNanosecond.First(p => p.Key == 0).Value;

        double intersectionNanosecondsForMainHail = (xIntersectionPosition - mainHailAtZeroNanoseconds.X) / mainHail.VelocityX;
        double intersectionNanosecondsForHailToCompare = (xIntersectionPosition - hailToCompareAtZeroNanoseconds.X) / hailToCompare.VelocityX;
        
        if (intersectionNanosecondsForMainHail > 0 && intersectionNanosecondsForHailToCompare > 0 && xIntersectionPosition is >= TestAreaStartingX and <= TestAreaEndingX && yIntersectionPosition is >= TestAreaStartingY and <= TestAreaEndingY)
        {
            Console.WriteLine($"Hail {mainHail.Id} will collide with Hail {hailToCompare.Id} at (X={xIntersectionPosition.ToString("F2")}, Y={yIntersectionPosition.ToString("F2")}).");
            return true;
        }

        return false;
    }
}
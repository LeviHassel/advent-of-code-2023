namespace AdventOfCode2023.Day10;

// https://adventofcode.com/2023/day/10

internal static class Part1
{
    public static void Run(string[] lines)
    {
        // Find starting pipe
        var step = GetFirstStep(lines);

        // Find first connector to starting pipe
        var directionsToTry = new List<Direction> { Direction.Right, Direction.Down, Direction.Left, Direction.Up};

        foreach (var direction in directionsToTry)
        {
            step.DirectionHeading = direction;
            var secondPointCoordinates = GetNextPointCoordinates(step);
            var secondPoint = lines[secondPointCoordinates.Item1][secondPointCoordinates.Item2];
            step.DirectionHeading = GetNextDirection(secondPoint, step.DirectionHeading);

            if (step.DirectionHeading != Direction.Unknown)
            {
                step.RowNumber = secondPointCoordinates.Item1;
                step.ColumnNumber = secondPointCoordinates.Item2;
                step.StepCount++;
                break;
            }
        }
        
        // Count the steps all the way back to the starting pipe
        while (step.DirectionHeading != Direction.Any && step.DirectionHeading != Direction.Unknown)
        {
            var nextPointCoordinates = GetNextPointCoordinates(step);
            var nextPoint = lines[nextPointCoordinates.Item1][nextPointCoordinates.Item2];

            step.DirectionHeading = GetNextDirection(nextPoint, step.DirectionHeading);
            step.RowNumber = nextPointCoordinates.Item1;
            step.ColumnNumber = nextPointCoordinates.Item2;
            step.StepCount++;
            Console.WriteLine($"Step {step.StepCount}: [{step.RowNumber}, {step.ColumnNumber}] heading {step.DirectionHeading.ToString().ToLower()}");
        }
        
        var stepsToFarthestPoint = step.StepCount / 2;
        Console.WriteLine($"Steps to farthest point: {stepsToFarthestPoint}");
    }

    public static Step GetFirstStep(string[] lines)
    {
        var startingRow = 0;
        var startingColumn = 0;
        
        for (var i=0; i < lines.Length; i++)
        {
            var line = lines[i];
            
            for (var j=0; j < line.Length; j++)
            {
                var character = line[j];
                
                if (character == 'S')
                {
                    startingRow = i;
                    startingColumn = j;
                }
            }
        }
        
        return new Step
        {
            RowNumber = startingRow,
            ColumnNumber = startingColumn,
            DirectionHeading = Direction.Any,
            StepCount = 0
        };
    }

    public static Direction GetNextDirection(char nextPoint, Direction directionHeading)
    {
        switch (nextPoint)
        {
            case 'S': // starting position of the animal; there is a pipe on this tile, but your sketch doesn't show what shape the pipe has
                return Direction.Any;
            case '|': // vertical pipe connecting north and south
                switch (directionHeading)
                {
                    case Direction.Down:
                        return Direction.Down;
                    case Direction.Up:
                        return Direction.Up;
                }
                break;
            case '-': // horizontal pipe connecting east and west
                switch (directionHeading)
                {
                    case Direction.Right:
                        return Direction.Right;
                    case Direction.Left:
                        return Direction.Left;
                }
                break;
            case 'L': // 90-degree bend connecting north and east
                switch (directionHeading)
                {
                    case Direction.Down:
                        return Direction.Right;
                    case Direction.Left:
                        return Direction.Up;
                }
                break;
            case 'J': // 90-degree bend connecting north and west
                switch (directionHeading)
                {
                    case Direction.Down:
                        return Direction.Left;
                    case Direction.Right:
                        return Direction.Up;
                }
                break;
            case '7': // 90-degree bend connecting south and west
                switch (directionHeading)
                {
                    case Direction.Right:
                        return Direction.Down;
                    case Direction.Up:
                        return Direction.Left;
                }
                break;
            case 'F': // 90-degree bend connecting south and east
                switch (directionHeading)
                {
                    case Direction.Left:
                        return Direction.Down;
                    case Direction.Up:
                        return Direction.Right;
                }
                break;
            case '.': // ground; there is no pipe in this tile
                return Direction.Unknown;
            default:
                return Direction.Unknown;
        }
        
        return Direction.Unknown;
    }

    public static Tuple<int, int> GetNextPointCoordinates(Step step)
    {
        var nextRowNumber = 0;
        var nextColumnNumber = 0;

        switch (step.DirectionHeading)
        {
            case Direction.Right:
                nextRowNumber = step.RowNumber;
                nextColumnNumber = step.ColumnNumber + 1;
                break;
            case Direction.Down:
                nextRowNumber = step.RowNumber + 1;
                nextColumnNumber = step.ColumnNumber;
                break;
            case Direction.Left:
                nextRowNumber = step.RowNumber;
                nextColumnNumber = step.ColumnNumber - 1;
                break;
            case Direction.Up:
                nextRowNumber = step.RowNumber - 1;
                nextColumnNumber = step.ColumnNumber;
                break;
            case Direction.Any:
                break;
        }

        return new Tuple<int, int>(nextRowNumber, nextColumnNumber);
    }
}
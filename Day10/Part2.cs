namespace AdventOfCode2023.Day10;

// https://adventofcode.com/2023/day/10

internal static class Part2
{
    public static void Run(string[] lines)
    {
        // Find starting pipe
        var path = new List<Step> { GetFirstStep(lines) };

        // Find first connector to starting pipe
        var directionsToTry = new List<Direction> { Direction.Right, Direction.Down, Direction.Left, Direction.Up};

        foreach (var direction in directionsToTry)
        {
            var step = path.First();
            
            step.DirectionHeading = direction;
            var secondPointCoordinates = GetNextPointCoordinates(step);
            var secondPoint = lines[secondPointCoordinates.Item1][secondPointCoordinates.Item2];
            var nextDirection = GetNextDirection(secondPoint, step.DirectionHeading);

            if (nextDirection != Direction.Unknown)
            {
                path.Add(new Step
                {
                    RowNumber = secondPointCoordinates.Item1,
                    ColumnNumber = secondPointCoordinates.Item2,
                    DirectionHeading = nextDirection,
                    StepCount = 1,
                });

                break;
            }
        }
        
        // Count the steps all the way back to the starting pipe
        while (path.Last().DirectionHeading != Direction.Any && path.Last().DirectionHeading != Direction.Unknown)
        {
            var step = path.Last();
            var nextPointCoordinates = GetNextPointCoordinates(step);
            var nextPoint = lines[nextPointCoordinates.Item1][nextPointCoordinates.Item2];
            var nextStep = new Step()
            {
                DirectionHeading = GetNextDirection(nextPoint, step.DirectionHeading),
                RowNumber = nextPointCoordinates.Item1,
                ColumnNumber = nextPointCoordinates.Item2,
                StepCount = step.StepCount += 1
            };
            
            path.Add(nextStep);

            Console.WriteLine($"Step {nextStep.StepCount}: [{nextStep.RowNumber}, {nextStep.ColumnNumber}] heading {nextStep.DirectionHeading.ToString().ToLower()}");
        }
        
        var tilesEnclosedByLoop = GetTilesEnclosedByLoop(path, lines);
        Console.WriteLine($"Tiles enclosed by loop: {tilesEnclosedByLoop}");
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

    public static int GetTilesEnclosedByLoop(List<Step> loopSteps, string[] lines)
    {
        var rowCount = lines.Length;
        var enclosedTileCount = 0;
        var tilesEnclosedOnRow = new List<int>();

        loopSteps = loopSteps.OrderBy(s => s.ColumnNumber).ToList();
        
        for (var y=0; y < rowCount; y++)
        {
            var line = lines[y];
            var stepsOnLine = loopSteps.Where(s => s.RowNumber == y).ToList();
            
            var insideBorder = false;
            Step? lastStep = null;
            var isLastStepStartOfCorner = false;

            foreach (var step in stepsOnLine)
            {
                var stepCharacter = line[step.ColumnNumber];

                var isStepStartOfCorner = false;
                var isStepEndOfCorner = false;
                var crossedBorder = false;
                
                switch (stepCharacter)
                {
                    case 'S': // starting position of the animal; there is a pipe on this tile, but your sketch doesn't show what shape the pipe has
                        isStepStartOfCorner = true;
                        isStepEndOfCorner = true;
                        break;
                    case '|': // vertical pipe connecting north and south
                        crossedBorder = true;
                        break;
                    case 'L': // 90-degree bend connecting north and east
                        isStepStartOfCorner = true;
                        break;
                    case 'J': // 90-degree bend connecting north and west
                        isStepEndOfCorner = true;
                        break;
                    case '7': // 90-degree bend connecting south and west
                        isStepEndOfCorner = true;
                        break;
                    case 'F': // 90-degree bend connecting south and east
                        isStepStartOfCorner = true;
                        break;
                    case '-':
                        continue;
                }
                
                // Start of corner: F, L, S
                if (lastStep != null && isLastStepStartOfCorner)
                {
                    // End of corner: 7, J, S
                    if (isStepEndOfCorner)
                    {
                        var lastStepCharacter = line[lastStep.ColumnNumber];
                        var startOfCornerGoesDown = lastStepCharacter is 'F' or '7' or 'S';
                        var endOfCornerGoesDown = stepCharacter is 'F' or '7' or 'S';

                        crossedBorder = startOfCornerGoesDown != endOfCornerGoesDown;
                    }
                    else
                    {
                        crossedBorder = true;
                    }
                }

                isLastStepStartOfCorner = isStepStartOfCorner;

                var prevInsideBorder = insideBorder;

                if (insideBorder && lastStep != null && step.ColumnNumber - lastStep.ColumnNumber > 1)
                {
                    var loopStepsInRange = loopSteps.Count(s => s.RowNumber == y && s.ColumnNumber >= lastStep.ColumnNumber && s.ColumnNumber <= step.ColumnNumber);

                    var tilesEnclosed = step.ColumnNumber - lastStep.ColumnNumber + 1 - loopStepsInRange;

                    if (tilesEnclosed > 0)
                    {
                        for (var x = lastStep.ColumnNumber; x < step.ColumnNumber; x++)
                        {
                            if (!loopSteps.Any(s => s.RowNumber == y && s.ColumnNumber == x))
                            {
                                tilesEnclosedOnRow.Add(x);
                            }
                        }
                    }
                    
                    enclosedTileCount += tilesEnclosed;
                }
                
                if (crossedBorder)
                {
                    insideBorder = !prevInsideBorder;
                }
                
                lastStep = step;
            }

            var lineToPrint = line.ToCharArray();
            
            foreach (var tileX in tilesEnclosedOnRow)
            {
                lineToPrint[tileX] = 'X';
            }
            
            Console.WriteLine(new string(lineToPrint));

            tilesEnclosedOnRow = new List<int>();
        }

        return enclosedTileCount;
    }
}
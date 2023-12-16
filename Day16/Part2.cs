namespace AdventOfCode2023.Day16;

// https://adventofcode.com/2023/day/16

internal static class Part2
{
    public static void Run(string[] lines)
    {
        var maxEnergizedTileCount = 0;

        var possibleStartingBeams = GetStartingBeams(lines);

        foreach (var beam in possibleStartingBeams)
        {
            var energizedTileCount = GetEnergizedTilesForStartingBeam(beam, lines);

            if (energizedTileCount > maxEnergizedTileCount)
            {
                maxEnergizedTileCount = energizedTileCount;
            }
        }
        
        Console.WriteLine($"Max energized tiles: {maxEnergizedTileCount}");
    }

    public static List<Beam> GetStartingBeams(string[] lines)
    {
        var maxX = lines.First().Length - 1;
        var maxY = lines.Length - 1;
        
        var possibleStartingBeams = new List<Beam>();

        // Add starting beams along left and right edges
        for (var i=0; i <= maxY; i++)
        {
            var leftBeam = new Beam()
            {
                Direction = Direction.Right,
                X = 0,
                Y = i,
            };
            
            var rightBeam = new Beam()
            {
                Direction = Direction.Left,
                X = maxX,
                Y = i,
            };
            
            possibleStartingBeams.Add(leftBeam);
            possibleStartingBeams.Add(rightBeam);
        }
        
        // Add starting beams along top and bottom edges
        for (var i=0; i <= maxX; i++)
        {
            var topBeam = new Beam()
            {
                Direction = Direction.Down,
                X = i,
                Y = 0,
            };
            
            var bottomBeam = new Beam()
            {
                Direction = Direction.Up,
                X = i,
                Y = maxY,
            };
            
            possibleStartingBeams.Add(topBeam);
            possibleStartingBeams.Add(bottomBeam);
        }

        return possibleStartingBeams;
    }

    public static int GetEnergizedTilesForStartingBeam(Beam startingBeam, string[] lines)
    {
        var maxX = lines.First().Length - 1;
        var maxY = lines.Length - 1;
        var energizedTileCount = 0;

        var energizedTileHistory = new List<Beam>
        {
            new()
            {
                X = startingBeam.X,
                Y = startingBeam.Y,
                Direction = startingBeam.Direction
            }
        };
        
        var beams = new List<Beam>
        {
            new()
            {
                X = startingBeam.X,
                Y = startingBeam.Y,
                Direction = startingBeam.Direction,
            }
        };

        while (beams.Any())
        {
            for (var i=0; i < beams.Count; i++)
            {
                var beam = beams[i];

                var tile = lines[beam.Y][beam.X];

                switch (tile)
                {
                    case '.':
                        // If the beam encounters empty space (.), it continues in the same direction.
                        break;
                    case '/':
                        // If the beam encounters a mirror (/ or \), the beam is reflected 90 degrees depending on the angle of the mirror. For instance, a rightward-moving beam that encounters a / mirror would continue upward in the mirror's column, while a rightward-moving beam that encounters a \ mirror would continue downward from the mirror's column.
                        switch (beam.Direction)
                        {
                            case Direction.Right:
                                beam.Direction = Direction.Up;
                                break;
                            case Direction.Down:
                                beam.Direction = Direction.Left;
                                break;
                            case Direction.Left:
                                beam.Direction = Direction.Down;
                                break;
                            case Direction.Up:
                                beam.Direction = Direction.Right;
                                break;
                        }
                        break;
                    case '\\':
                        // If the beam encounters a mirror (/ or \), the beam is reflected 90 degrees depending on the angle of the mirror. For instance, a rightward-moving beam that encounters a / mirror would continue upward in the mirror's column, while a rightward-moving beam that encounters a \ mirror would continue downward from the mirror's column.
                        switch (beam.Direction)
                        {
                            case Direction.Right:
                                beam.Direction = Direction.Down;
                                break;
                            case Direction.Down:
                                beam.Direction = Direction.Right;
                                break;
                            case Direction.Left:
                                beam.Direction = Direction.Up;
                                break;
                            case Direction.Up:
                                beam.Direction = Direction.Left;
                                break;
                        }
                        break;
                    case '|':
                        if (beam.Direction is Direction.Right or Direction.Left)
                        {
                            // If the beam encounters the flat side of a splitter (| or -), the beam is split into two beams going in each of the two directions the splitter's pointy ends are pointing. For instance, a rightward-moving beam that encounters a | splitter would split into two beams: one that continues upward from the splitter's column and one that continues downward from the splitter's column.
                            beam.Direction = Direction.Up;

                            var splitBeam = new Beam()
                            {
                                X = beam.X,
                                Y = beam.Y,
                                Direction = Direction.Down
                            };
                            
                            beams.Add(splitBeam);
                        }
                        // If the beam encounters the pointy end of a splitter (| or -), the beam passes through the splitter as if the splitter were empty space. For instance, a rightward-moving beam that encounters a - splitter would continue in the same direction.
                        break;
                    case '-':
                        if (beam.Direction is Direction.Up or Direction.Down)
                        {
                            // If the beam encounters the flat side of a splitter (| or -), the beam is split into two beams going in each of the two directions the splitter's pointy ends are pointing. For instance, a rightward-moving beam that encounters a | splitter would split into two beams: one that continues upward from the splitter's column and one that continues downward from the splitter's column.
                            beam.Direction = Direction.Right;

                            var splitBeam = new Beam()
                            {
                                X = beam.X,
                                Y = beam.Y,
                                Direction = Direction.Left
                            };
                            
                            beams.Add(splitBeam);
                        }
                        // If the beam encounters the pointy end of a splitter (| or -), the beam passes through the splitter as if the splitter were empty space. For instance, a rightward-moving beam that encounters a - splitter would continue in the same direction.
                        break;
                }
                
                switch (beam.Direction)
                {
                    case Direction.Right:
                        beam.X += 1;
                        break;
                    case Direction.Down:
                        beam.Y += 1;
                        break;
                    case Direction.Left:
                        beam.X -= 1;
                        break;
                    case Direction.Up:
                        beam.Y -= 1;
                        break;
                }
                
                if (beam.X < 0 ||  beam.X > maxX || beam.Y < 0 || beam.Y > maxY)
                {
                    beams.Remove(beam);
                    continue;
                }

                if (!energizedTileHistory.Any(t => t.X == beam.X && t.Y == beam.Y && t.Direction == beam.Direction))
                {
                    energizedTileHistory.Add(new()
                    {
                        X = beam.X,
                        Y = beam.Y,
                        Direction = beam.Direction
                    });

                    energizedTileCount = energizedTileHistory.DistinctBy(t => (t.X, t.Y)).Count();

                    Console.WriteLine($"Energized tiles for starting beam at ({startingBeam.X}, {startingBeam.Y}) going {startingBeam.Direction}: {energizedTileCount} ({beams.Count} current beams)");
                }
                else
                {
                    beams.Remove(beam);
                }
            }
        }

        return energizedTileCount;
    }
}
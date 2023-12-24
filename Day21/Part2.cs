namespace AdventOfCode2023.Day21;

// https://adventofcode.com/2023/day/21

internal static class Part2
{
    public static void Run(string[] lines)
    {
        var totalDesiredStepsToTake = 26501365;
        var countEvenSteps = totalDesiredStepsToTake % 2 == 0;
        var allowExplorationOfTheInfinite = true;

        var startingY = lines.ToList().FindIndex(l => l.Contains('S'));
        var startingX = lines[startingY].IndexOf('S');
        var maxY = lines.Length - 1;
        var maxX = lines.First().Length - 1;

        var mapFillData = new Dictionary<(int, int), MapFillData>();
        var plotQueue = new Queue<PlotQueueMessage>();
        var exploredPlotsForEnd = new Dictionary<string, PlotLocation>(); // counts only even or odd steps depending on totalDesiredStepsToTake
        var fullyExploredMaps = new Dictionary<(int, int), long>(); // counts only even or odd steps depending on totalDesiredStepsToTake
        var plotsThatShouldNotBeExplored = new Dictionary<string, PlotLocation>();
        long highestStepCountSoFar = 0;

        var startingLocation = new PlotLocation()
        {
            MapX = 0,
            MapY = 0,
            X = startingX,
            Y = startingY
        };

        plotQueue.Enqueue(new PlotQueueMessage
        {
            Location = startingLocation,
            StepCount = 0
        });

        while (mapFillData.Count < 250)
        {
            var plotQueueMessage = plotQueue.Dequeue();

            var location = plotQueueMessage.Location;
            var currentX = location.X;
            var currentY = location.Y;
            var currentStepCount = plotQueueMessage.StepCount;
            var mapX = location.MapX;
            var mapY = location.MapY;
            var currentStepEven = currentStepCount % 2 == 0;
            
            if (currentStepCount > highestStepCountSoFar && countEvenSteps != currentStepEven)
            {
                var stepToEvaluate = currentStepCount - 1;
                
                var incompleteMaps = mapFillData.Where(m => m.Value.StepsToComplete == -1).ToList();
                
                foreach (var incompleteMap in incompleteMaps)
                {
                    var exploredPlotsForMap = exploredPlotsForEnd.Where(m => m.Value.MapX == incompleteMap.Value.MapX && m.Value.MapY == incompleteMap.Value.MapY).ToList();
                    var currentExploredPlotCount = exploredPlotsForMap.Count;
                    var previouslyExploredPlots = incompleteMap.Value.StepsToExploredPlots;
                    
                    if (previouslyExploredPlots.Any() && currentExploredPlotCount == previouslyExploredPlots.Last().Value)
                    {
                        // this map is complete
                        incompleteMap.Value.StepsToComplete = previouslyExploredPlots.Last().Key;
                        
                        // once a map is fully explored, remove data on it from both lists
                        exploredPlotsForEnd = exploredPlotsForEnd.Where(p => !(p.Value.MapX == incompleteMap.Value.MapX && p.Value.MapY == incompleteMap.Value.MapY)).ToDictionary(p => p.Key, p => p.Value);
                        fullyExploredMaps.Add((incompleteMap.Value.MapX, incompleteMap.Value.MapY), currentExploredPlotCount);
                        
                        plotsThatShouldNotBeExplored = plotsThatShouldNotBeExplored.Where(p => !(p.Value.MapX == incompleteMap.Value.MapX && p.Value.MapY == incompleteMap.Value.MapY)).ToDictionary(p => p.Key, p => p.Value);

                        continue;
                    }
                    
                    var currentStepForMap = stepToEvaluate - incompleteMap.Value.FirstStep;
                    
                    previouslyExploredPlots.Add((int)currentStepForMap, currentExploredPlotCount);
                }

                highestStepCountSoFar = currentStepCount;
                
                if (currentStepCount % 10 == 0)
                {
                    Console.WriteLine($"Step count: {currentStepCount}, Map fill count: {mapFillData.Count}, Queue size: {plotQueue.Count}");
                }
            }

            if (ShouldPlotBeExplored(currentX, currentY, lines))
            {
                if (countEvenSteps == currentStepEven)
                {
                    exploredPlotsForEnd.Add(location.Id, location);
                }
                
                var newStepCount = currentStepCount + 1;

                // Enqueue left plot
                var leftX = currentX - 1;
                var leftMapX = mapX;

                if (leftX < 0)
                {
                    leftX = maxX;
                    leftMapX = mapX - 1;
                    TryAddMapData(mapFillData, leftMapX, mapY, leftX, currentY, newStepCount);
                }

                if (leftMapX == mapX || allowExplorationOfTheInfinite)
                {
                    EnqueuePlotMessage(leftX, currentY, leftMapX, mapY, newStepCount, plotQueue, exploredPlotsForEnd, plotsThatShouldNotBeExplored, fullyExploredMaps);
                }

                // Enqueue right plot
                var rightX = currentX + 1;
                var rightMapX = mapX;

                if (rightX > maxX)
                {
                    rightX = 0;
                    rightMapX = mapX + 1;
                    TryAddMapData(mapFillData, rightMapX, mapY, rightX, currentY, newStepCount);
                }

                if (rightMapX == mapX || allowExplorationOfTheInfinite)
                {
                    EnqueuePlotMessage(rightX, currentY, rightMapX, mapY, newStepCount, plotQueue, exploredPlotsForEnd, plotsThatShouldNotBeExplored, fullyExploredMaps);
                }

                // Enqueue up plot
                var upY = currentY - 1;
                var upMapY = mapY;

                if (upY < 0)
                {
                    upY = maxY;
                    upMapY = mapY - 1;
                    TryAddMapData(mapFillData, mapX, upMapY, currentX, upY, newStepCount);
                }

                if (upMapY == mapY || allowExplorationOfTheInfinite)
                {
                    EnqueuePlotMessage(currentX, upY, mapX, upMapY, newStepCount, plotQueue, exploredPlotsForEnd, plotsThatShouldNotBeExplored, fullyExploredMaps);
                }
                    
                // Enqueue down plot
                var downY = currentY + 1;
                var downMapY = mapY;

                if (downY > maxY)
                {
                    downY = 0;
                    downMapY = mapY + 1;
                    TryAddMapData(mapFillData, mapX, downMapY, currentX, downY, newStepCount);
                }

                if (downMapY == mapY || allowExplorationOfTheInfinite)
                {
                    EnqueuePlotMessage(currentX, downY, mapX, downMapY, newStepCount, plotQueue, exploredPlotsForEnd, plotsThatShouldNotBeExplored, fullyExploredMaps);
                }
            }
            else
            {
                if (!plotsThatShouldNotBeExplored.ContainsKey(location.Id))
                {
                    plotsThatShouldNotBeExplored.Add(location.Id, location);
                }
            }
        }
        
        // Identify patterns from map fill data
        var mapPatterns = new List<MapCreationPattern>();
        var mapCoordinatesWithoutPatterns = new List<Tuple<int, int>>();
        
        foreach (var map in mapFillData)
        {
            map.Value.GroupCreation = $"{map.Value.EntryX}.{map.Value.EntryY}.{map.Value.StepsToComplete}.{map.Value.StepsToExploredPlotsString}";
        }

        var mapsGroupedByEntryPoint = mapFillData.Where(d => d.Value.StepsToComplete != -1).GroupBy(m => (m.Value.GroupCreation)).ToList();
        var mapsWithPatterns = mapsGroupedByEntryPoint.Where(g => g.Count() > 1).ToList();
        var mapsWithoutPatterns = mapsGroupedByEntryPoint.Where(g => g.Count() <= 1).ToList();
        
        foreach (var entryPointGroup in mapsWithPatterns)
        {
            var mapsGroupedByStartingStep = entryPointGroup.GroupBy(g => g.Value.FirstStep).ToList();
            
            var newStepDifferencesBetweenPatterns = new Dictionary<long, long>(); // Key: startingStep, Value: Steps between patterns

            for (var i=0; i < mapsGroupedByStartingStep.Count - 1; i++)
            {
                var startingStepGroup = mapsGroupedByStartingStep[i];
                var nextStartingStepGroup = mapsGroupedByStartingStep[i + 1];
                
                var diff = nextStartingStepGroup.Key - startingStepGroup.Key;
                newStepDifferencesBetweenPatterns.Add(startingStepGroup.Key, diff);
            }

            var distinctStepDifferences = newStepDifferencesBetweenPatterns.GroupBy(d => d.Value).Where(g => g.Count() > 1).ToArray();
            var distinctStepDifferencesWithFewOccurrences = newStepDifferencesBetweenPatterns.GroupBy(d => d.Value).Where(g => g.Count() <= 1).ToArray();
            
            // Ensure any plots not in the patterns get added to the total at the end
            foreach (var distinctStepDifferenceGroup in distinctStepDifferencesWithFewOccurrences)
            {
                foreach (var stepDifference in distinctStepDifferenceGroup)
                {
                    var maps = mapsGroupedByStartingStep.Where(g => g.Key == stepDifference.Key).ToList();
                    mapCoordinatesWithoutPatterns.AddRange(maps.SelectMany(g => g.Select(m => new Tuple<int, int>(m.Value.MapX, m.Value.MapY))).ToList());
                }
            }

            // Create patterns for everything else
            foreach (var distinctStepDifference in distinctStepDifferences)
            {
                var firstGroupStepCount = distinctStepDifference.ToArray()[0].Key;
                var secondGroupStepCount = distinctStepDifference.ToArray()[1].Key;

                var stepDiffBetweenPair = secondGroupStepCount - firstGroupStepCount;
                
                var firstGroup = mapsGroupedByStartingStep.First(g => g.Key == firstGroupStepCount);
                var secondGroup = mapsGroupedByStartingStep.First(g => g.Key == secondGroupStepCount);
                
                var mapsCreatedDiffBetweenFirstPair = secondGroup.Count() - firstGroup.Count();

                var mapPattern = new MapCreationPattern
                {
                    FirstStep = firstGroupStepCount,
                    StepsBetween = stepDiffBetweenPair,
                    EntryX = firstGroup.First().Value.EntryX,
                    EntryY = firstGroup.First().Value.EntryY,
                    MinMapX = firstGroup.First().Value.MapX,
                    MinMapY = firstGroup.First().Value.MapY,
                    MapsCreatedFirstTime = firstGroup.Count(),
                    AdditionalMapsCreatedEachTime = mapsCreatedDiffBetweenFirstPair,
                    StepsToComplete = firstGroup.First().Value.StepsToComplete,
                    StepsToExploredPlots = firstGroup.First().Value.StepsToExploredPlots
                };
                    
                mapPatterns.Add(mapPattern);
            }
        }
        
        mapCoordinatesWithoutPatterns.AddRange(mapsWithoutPatterns.SelectMany(g => g.Select(m => new Tuple<int, int>(m.Value.MapX, m.Value.MapY))).ToList());

        Console.WriteLine(string.Join(',', mapPatterns.Select(p => $"Pattern starting at ({p.MinMapX}, {p.MinMapY})")));
        Console.WriteLine(string.Join(',', mapCoordinatesWithoutPatterns.Select(p => $"Unmatched map at ({p.Item1}, {p.Item2})")));
        
        long plotsByEnd = totalDesiredStepsToTake;
        
        // Count explored plots for each map pattern
        foreach (var mapPattern in mapPatterns)
        {
            var maxStepsToComplete = mapPattern.StepsToComplete;
            long plotsFilledByEnd = mapPattern.StepsToExploredPlots.Last().Value;

            var mapsByEnd = new List<MapDataForEnd>();
            var currentMapsToAdd = mapPattern.MapsCreatedFirstTime;

            for (var i= mapPattern.FirstStep; i <= totalDesiredStepsToTake; i+= mapPattern.StepsBetween)
            {
                mapsByEnd.Add(new MapDataForEnd
                {
                    Count = currentMapsToAdd,
                    FirstStep = i,
                    StepsRemainingAfterStart = totalDesiredStepsToTake - i
                });
                currentMapsToAdd += mapPattern.AdditionalMapsCreatedEachTime;
            }

            var completeMaps = mapsByEnd.Where(m => m.StepsRemainingAfterStart >= maxStepsToComplete);
            var incompleteMaps = mapsByEnd.Where(m => m.StepsRemainingAfterStart < maxStepsToComplete);

            long totalPlotsForCompleteMaps = 0;

            foreach (var completeMap in completeMaps)
            {
                totalPlotsForCompleteMaps += (long)completeMap.Count * (long)plotsFilledByEnd;
            }
            
            long totalPlotsForIncompleteMaps = 0;

            foreach (var incompleteMap in incompleteMaps)
            {
                long totalPlotsForIncompleteMap = 0;

                var foundStep = mapPattern.StepsToExploredPlots.TryGetValue((int)incompleteMap.StepsRemainingAfterStart, out var plotsExplored);

                if (foundStep)
                {
                    totalPlotsForIncompleteMap = incompleteMap.Count * plotsExplored;
                }
                else
                {
                    mapPattern.StepsToExploredPlots.TryGetValue((int)incompleteMap.StepsRemainingAfterStart - 1, out var plotsExploredOneEarlier);
                    totalPlotsForIncompleteMap = incompleteMap.Count * plotsExploredOneEarlier;
                }
                
                totalPlotsForIncompleteMaps += totalPlotsForIncompleteMap;
            }

            long totalPlotsForPattern = totalPlotsForCompleteMaps + totalPlotsForIncompleteMaps;
            plotsByEnd += totalPlotsForPattern;
        }
        
        var fullMapsCoveredOutsideOfPatterns = fullyExploredMaps.Where(e => mapCoordinatesWithoutPatterns.Any(m => m.Item1 == e.Key.Item1 && m.Item2 == e.Key.Item2)).ToList();
        var plotCountFromFullyCoveredMaps = fullMapsCoveredOutsideOfPatterns.Sum(m => m.Value);
        
        plotsByEnd += plotCountFromFullyCoveredMaps - totalDesiredStepsToTake;
        
        Console.WriteLine($"Garden plots that the elf could reach in {totalDesiredStepsToTake} steps: {plotsByEnd}");
    }

    public static void TryAddMapData(Dictionary<(int, int), MapFillData> mapFillData, int mapX, int mapY, int x, int y, long currentStepCount)
    {
        if (!mapFillData.ContainsKey((mapX, mapY)))
        {
            var mapFillDataEntry = new MapFillData
            {
                MapX = mapX,
                MapY = mapY,
                EntryX = x,
                EntryY = y,
                StepsToExploredPlots = new Dictionary<int, int>(),
                FirstStep = currentStepCount,
                StepsToComplete = -1
            };
            
            mapFillData.Add((mapX, mapY), mapFillDataEntry);
        }
    }
    
    public static void EnqueuePlotMessage(int x, int y, int mapX, int mapY, long currentStepCount, Queue<PlotQueueMessage> plotQueue, Dictionary<string, PlotLocation> exploredPlotByEnd, Dictionary<string, PlotLocation> plotsThatShouldNotBeExplored, Dictionary<(int, int), long> fullyExploredMaps)
    {
        var location = new PlotLocation()
        {
            MapX = mapX,
            MapY = mapY,
            X = x,
            Y = y
        };

        var queueMessage = new PlotQueueMessage()
        {
            Location = location,
            StepCount = currentStepCount
        };
                    
        if (!plotQueue.Any(m => m.Location.Id == location.Id) && !fullyExploredMaps.ContainsKey((mapX, mapY)) && !exploredPlotByEnd.ContainsKey(location.Id) && !plotsThatShouldNotBeExplored.ContainsKey(location.Id))
        {
            plotQueue.Enqueue(queueMessage);
        }
    }

    public static bool ShouldPlotBeExplored(int x, int y, string[] lines)
    {
        if (lines[y][x] == '#')
        {
            return false;
        }

        return true;
    }
}

class PlotQueueMessage
{
    public PlotLocation Location;
    public long StepCount;
}

class PlotLocation
{
    public string Id => $"{MapX}.{MapY}.{X}.{Y}";
    public int X;
    public int Y;
    public int MapX;
    public int MapY;
}

public class MapFillData {
    public int MapX;
    public int MapY;
    public int EntryX;
    public int EntryY;
    public Dictionary<int, int> StepsToExploredPlots;
    public long FirstStep;
    public int StepsToComplete;
    public string GroupCreation;
    public string StepsToExploredPlotsString => string.Join(".", StepsToExploredPlots.Select(s => $"{s.Key}_{s.Value}"));
}

public class MapCreationPattern
{
    public long FirstStep;
    public long StepsBetween;
    public int EntryX;
    public int EntryY;
    public int MinMapX;
    public int MinMapY;
    public int StepsToComplete;
    public Dictionary<int, int> StepsToExploredPlots;
    public int MapsCreatedFirstTime;
    public int AdditionalMapsCreatedEachTime;
    public bool DoubleChecked;
}

public class MapDataForEnd {
    public int Count;
    public long FirstStep;
    public long StepsRemainingAfterStart;
}
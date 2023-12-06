namespace AdventOfCode2023.Day5;

// ReSharper disable once UnusedType.Global
internal static class Part2
{
    public static void Run(string[] lines)
    {
        var maps = new List<SourceDestinationMap>();

        SourceDestinationMap currentMap = null;

        var seedRanges = new List<SeedRange>();
        
        foreach (var line in lines)
        {
            if (line.Contains("seeds: "))
            {
                var seeds = line.Split(' ').ToList();

                // Get rid of `seeds:`
                seeds.RemoveAt(0);

                for (var i = 0; i < seeds.Count; i += 2)
                {
                    var rangeStart = long.Parse(seeds[i]);
                    var rangeLength = long.Parse(seeds[i + 1]);
                    var rangeEnd = rangeStart + rangeLength - 1;
                    
                    seedRanges.Add(new SeedRange()
                    {
                        RangeStart = rangeStart,
                        RangeEnd = rangeEnd
                    });
                }

                continue;
            }

            if (line.Contains(" map:"))
            {
                var contents = line.Split(' ').ToList();
                var properties = contents[0].Split('-').ToList();
                
                currentMap = new SourceDestinationMap()
                {
                    SourceProperty = properties[0],
                    DestinationProperty = properties[2],
                    ConversionRanges = new List<ConversionRange>()
                };
                
                continue;
            }
            
            if (String.IsNullOrWhiteSpace(line))
            {
                // line is just whitespace
                if (currentMap != null)
                {
                    maps.Add(currentMap);
                    currentMap = null;
                }

                continue;
            }
            
            if (Char.IsDigit(line[0]))
            {
                var digits = line.Split(' ').ToList();
                var sourceRangeStart = long.Parse(digits[1]);
                var destinationRangeStart = long.Parse(digits[0]);
                var rangeLength = long.Parse(digits[2]);
                
                var conversionRange = new ConversionRange
                {
                    SourceRangeStart = sourceRangeStart,
                    SourceRangeEnd = sourceRangeStart + rangeLength - 1,
                    DestinationRangeStart = destinationRangeStart,
                    DestinationRangeEnd = destinationRangeStart + rangeLength - 1
                };

                if (currentMap != null)
                {
                    currentMap.ConversionRanges.Add(conversionRange);
                }
            }
        }
        
        if (currentMap != null)
        {
            maps.Add(currentMap);
        }
        
        // iterate through lowest possible location values, find the seed numbers that correspond with them.
        var locationDestinationMap = maps.First(m => m.DestinationProperty == "location");
        
        var orderedLocationRanges = locationDestinationMap.ConversionRanges.OrderBy(cr => cr.DestinationRangeStart);
        var maxLocation = orderedLocationRanges.Max(olr => olr.DestinationRangeEnd);
        maps.Reverse();

        for (long currentLocation = 0; currentLocation < maxLocation; currentLocation++)
        {
            var path = new Dictionary<string, long>();
            path.Add("location", currentLocation);

            foreach (var map in maps)
            {
                var destinationProperty = path.First(p => p.Key == map.DestinationProperty);
                var sourceValue = destinationProperty.Value;

                var matchingRange = map.ConversionRanges.FirstOrDefault(cv =>
                    cv.DestinationRangeStart <= destinationProperty.Value && cv.DestinationRangeEnd >= destinationProperty.Value);
                    
                if (matchingRange != null)
                {
                    var diff = destinationProperty.Value - matchingRange.DestinationRangeStart;
                    sourceValue = matchingRange.SourceRangeStart + diff;
                }
                    
                path.Add(map.SourceProperty, sourceValue);
            }

            var currentPotentialSeed = path["seed"];

            foreach (var seedRange in seedRanges)
            {
                if (currentPotentialSeed <= seedRange.RangeEnd && currentPotentialSeed >= seedRange.RangeStart)
                {
                    Console.WriteLine(currentLocation);
                    return;
                }
            }
            
            Console.WriteLine($"Not {currentLocation}");
        }
    }

    public class SeedRange
    {
        public long RangeStart;
        public long RangeEnd;
    }

    public class SourceDestinationMap
    {
        public string SourceProperty;
        public string DestinationProperty;
        public List<ConversionRange> ConversionRanges;
    }

    public class ConversionRange
    {
        public long SourceRangeStart;
        public long SourceRangeEnd;
        public long DestinationRangeStart;
        public long DestinationRangeEnd;
    }
}
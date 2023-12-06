namespace AdventOfCode2023.Day5;

// ReSharper disable once UnusedType.Global
internal static class Part1
{
    public static void Run(string[] lines)
    {
        var paths = new List<Dictionary<string, long>>();
        var maps = new List<SourceDestinationMap>();
        
        SourceDestinationMap currentMap = null;
        
        foreach (var line in lines)
        {
            if (line.Contains("seeds: "))
            {
                var seeds = line.Split(' ').ToList();

                // Get rid of `seeds:`
                seeds.RemoveAt(0);
                
                foreach (var seed in seeds)
                {
                    var path = new Dictionary<string, long>();
                    path.Add("seed", long.Parse(seed));
                    paths.Add(path);
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
                    SourceRangeEnd = sourceRangeStart + rangeLength,
                    DestinationRangeStart = destinationRangeStart,
                    DestinationRangeEnd = destinationRangeStart + rangeLength
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

        foreach (var path in paths)
        {
            foreach (var map in maps)
            {
                var sourceProperty = path.First(p => p.Key == map.SourceProperty);

                var destinationValue = sourceProperty.Value;

                var matchingRange = map.ConversionRanges.FirstOrDefault(cv =>
                    cv.SourceRangeStart <= sourceProperty.Value && cv.SourceRangeEnd >= sourceProperty.Value);
                
                if (matchingRange != null)
                {
                    var diff = sourceProperty.Value - matchingRange.SourceRangeStart;
                    destinationValue = matchingRange.DestinationRangeStart + diff;
                }
                
                path.Add(map.DestinationProperty, destinationValue);
            }
        }

        var minLocation = paths.Min(p => p["location"]);

        Console.WriteLine(minLocation);
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
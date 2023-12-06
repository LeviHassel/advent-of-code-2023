namespace AdventOfCode2023.Day1;

// ReSharper disable once UnusedType.Global
internal static class Part2
{
    public static void Run(string[] lines)
    {
        int runningTotal = 0;
        
        foreach (var line in lines)
        {
            var lineLowered = line.ToLower();
            
            var numbers = new Dictionary<string, int>()
            {
                {"one", 1},
                {"two", 2},
                {"three", 3},
                {"four", 4},
                {"five", 5},
                {"six", 6},
                {"seven", 7},
                {"eight", 8},
                {"nine", 9}
            };

            int firstWordIndex = -1;
            var firstWordValue = -1;
            int lastWordIndex = -1;
            var lastWordValue = -1;

            foreach (var number in numbers)
            {
                var firstIndex = lineLowered.IndexOf(number.Key);
                var lastIndex = lineLowered.LastIndexOf(number.Key);
                
                if (firstIndex > -1)
                {
                    if (firstWordIndex == -1 || firstIndex < firstWordIndex)
                    {
                        firstWordIndex = firstIndex;
                        firstWordValue = number.Value;
                    }
                    if (lastWordIndex == -1 || lastIndex > lastWordIndex)
                    {
                        lastWordIndex = lastIndex;
                        lastWordValue = number.Value;
                    }
                }
            }

            char firstChar = '?';
            char lastChar = '?';
            int firstNumber = -1;
            int lastNumber = -1;
            var firstNumberIndex = -1;
            var lastNumberIndex = -1;

            if (line.Any(x => int.TryParse(x.ToString(), out var blah)))
            {
                firstChar = line.First(x => int.TryParse(x.ToString(), out var blah));
                lastChar = line.Last(x => int.TryParse(x.ToString(), out var blah));
                
                firstNumber = int.Parse(firstChar.ToString());
                lastNumber = int.Parse(lastChar.ToString());
                
                firstNumberIndex = line.IndexOf(firstChar);
                lastNumberIndex = line.LastIndexOf(lastChar);
            }
            
            Console.WriteLine(line);
            Console.WriteLine($"First number: {firstNumber}");
            Console.WriteLine($"Last number: {lastNumber}");
            Console.WriteLine($"First word: {firstWordValue}");
            Console.WriteLine($"Last word: {lastWordValue}");
            
            int firstInput = -1;
            int lastInput = -1;

            if (firstNumber > -1)
            {
                firstInput = firstNumber;
            }
            
            if (lastNumber > -1)
            {
                lastInput = lastNumber;
            }

            if (firstWordIndex > -1)
            {
                firstInput = firstWordValue;
            }

            if (lastWordIndex > -1)
            {
                lastInput = lastWordValue;
            }
            
            if (firstWordIndex > -1 && firstNumber > -1)
            {
                firstInput = firstNumberIndex < firstWordIndex ? firstNumber : firstWordValue;
            }

            if (lastWordIndex > -1 && lastNumber > -1)
            {
                lastInput = lastNumberIndex > lastWordIndex ? lastNumber : lastWordValue;
            }

            var lineTotal = $"{firstInput}{lastInput}";
            int.TryParse(lineTotal, out var lineTotalNum);
            Console.WriteLine(lineTotalNum);
            runningTotal += lineTotalNum;
        }

        Console.WriteLine(runningTotal);
    }
}
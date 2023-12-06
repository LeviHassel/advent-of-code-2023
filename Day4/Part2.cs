namespace AdventOfCode2023.Day4;

// ReSharper disable once UnusedType.Global
internal static class Part2
{
    public static void Run(string[] lines)
    {
        var copiedScratchcards = new List<string>();
        var originalScratchcardCount = lines.Length;
        
        var scratchcardTotal = 0;

        for (var i=0; i < originalScratchcardCount; i++)
        {
            var line = lines[i];
            
            var lineMatchCount = 0;
            var copiedScratchcardsForLine = copiedScratchcards.RemoveAll(s => s.Contains($"{i + 1}:"));
            var scratchcardQuantity = 1 + copiedScratchcardsForLine;
            
            var titleContentsSplit = line.Split(':');
            var contents = titleContentsSplit[1].Trim().Split('|');
            var winningNumbers = contents[0].Trim().Split(' ');
            var yourNumbers = contents[1].Trim().Split(' ');
            
            foreach (var winningNumber in winningNumbers)
            {
                if (String.IsNullOrWhiteSpace(winningNumber))
                {
                    continue;
                }
                
                if (yourNumbers.Contains(winningNumber))
                {
                    lineMatchCount++;
                }
            }

            Console.WriteLine(line);
            Console.WriteLine($"lineMatchCount: {lineMatchCount}");
            Console.WriteLine($"scratchcardQuantity: {scratchcardQuantity}");

            for (var j = 1; j <= lineMatchCount; j++)
            {
                var copiedScratchcardIndex = i + j;
                
                if (copiedScratchcardIndex >= originalScratchcardCount)
                {
                    break;
                }

                for (var k = 0; k < scratchcardQuantity; k++)
                {
                    copiedScratchcards.Add(lines[copiedScratchcardIndex]);
                }
            }
            
            scratchcardTotal += scratchcardQuantity;
        }
        
        Console.WriteLine(scratchcardTotal);
    }
}
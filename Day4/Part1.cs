namespace AdventOfCode2023.Day4;

// https://adventofcode.com/2023/day/4

internal static class Part1
{
    public static void Run(string[] lines)
    {
        var scorecardTotal = 0;
        
        foreach (var line in lines)
        {
            var lineTotal = 0;
            
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
                
                Console.WriteLine(winningNumber);

                if (yourNumbers.Contains(winningNumber))
                {
                    lineTotal = lineTotal == 0 ? 1 : lineTotal * 2;
                }
            }

            Console.WriteLine(line);
            Console.WriteLine(lineTotal);

            scorecardTotal += lineTotal;
        }
        
        Console.WriteLine(scorecardTotal);
    }
}
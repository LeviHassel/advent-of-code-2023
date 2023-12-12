namespace AdventOfCode2023.Day9;

// https://adventofcode.com/2023/day/9

internal static class Part1
{
    public static void Run(string[] lines)
    {
        long totalSum = 0;
        
        foreach (var line in lines)
        {
            var sequences = new List<List<long>>();
            
            sequences.Add(line.Split(" ").Select(long.Parse).ToList());

            while (sequences.Last().Any(v => v != 0))
            {
                var lastSequence = sequences.Last();
                
                var diffSequence = new List<long>();

                for (var i = 1; i < lastSequence.Count; i++)
                {
                    diffSequence.Add(lastSequence[i] - lastSequence[i - 1]);
                }
                
                sequences.Add(diffSequence);
            }

            sequences.Reverse();
            
            sequences.First().Add(0);
            
            for (var i = 1; i < sequences.Count(); i++)
            {
                var sequence = sequences[i];
                var lastSequence = sequences[i - 1];

                long amountToAdd = sequence.Last() + lastSequence.Last();
                
                sequence.Add(amountToAdd);
            }

            totalSum += sequences.Last().Last();
        }
        
        Console.WriteLine(totalSum);
    }
}
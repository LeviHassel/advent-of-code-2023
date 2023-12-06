namespace AdventOfCode2023.Day3;

// https://adventofcode.com/2023/day/3

internal static class Part1
{
    public static void Run(string[] lines)
    {
        var partNumbers = new List<PartNumber>();
        var symbols = new List<Symbol>();
        
        var maxRowLength = lines.Max(l => l.Length);
        var lineCount = lines.Count();
        
        // Populate symbols and part numbers
        for (var i=0; i < lineCount; i++)
        {
            var line = lines[i];
            
            var rowPartNumbers = FindPartNumbersForRow(line, i);
            partNumbers.AddRange(rowPartNumbers);

            var rowSymbols = FindSymbolsForRow(line, i);
            symbols.AddRange(rowSymbols);
        }

        foreach (var partNumber in partNumbers)
        {
            var minRowNumber = partNumber.RowNumber - 1 >= 0 ? partNumber.RowNumber - 1 : 0;
            var maxRowNumber = partNumber.RowNumber + 1 <= lineCount - 1? partNumber.RowNumber + 1 : lineCount - 1;
            
            var minSymbolIndex = partNumber.StartIndex - 1 >= 0 ? partNumber.StartIndex - 1 : 0;
            var maxSymbolIndex = partNumber.EndIndex + 1 <= maxRowLength - 1 ? partNumber.EndIndex + 1 : maxRowLength - 1;
            
            var nearbySymbols = symbols.Any(s =>
                s.RowNumber <= maxRowNumber
                && s.RowNumber >= minRowNumber
                && s.Index <= maxSymbolIndex
                && s.Index >= minSymbolIndex);

            partNumber.IsActualPartNumber = nearbySymbols;
            
            Console.WriteLine(partNumber.Value);
            Console.WriteLine($"Row: {minRowNumber}-{maxRowNumber}, Index: {minSymbolIndex}-{maxSymbolIndex}");
            Console.WriteLine(nearbySymbols);
            Console.WriteLine("");
        }

        var validPartNumbers = partNumbers.Where(pn => pn.IsActualPartNumber).ToList();
        int totalSum = validPartNumbers.Sum(vpn => vpn.Value);
        Console.WriteLine(totalSum);
        
        // last time: 543410
    }

    public static List<PartNumber> FindPartNumbersForRow(string row, int rowNumber)
    {
        var partNumbers = new List<PartNumber>();

        PartNumber currPartNumber = null;

        for (int i=0; i < row.Length; i++)
        {
            var character = row[i];
            
            var isNumber = Char.IsNumber(character);

            if (isNumber)
            {
                if (currPartNumber == null)
                { 
                    currPartNumber = new PartNumber()
                    {
                        Value = int.Parse(character.ToString()),
                        RowNumber = rowNumber,
                        StartIndex = i,
                        EndIndex = i,
                        IsActualPartNumber = false
                    };
                }
                else
                {
                    currPartNumber.Value = int.Parse($"{currPartNumber.Value}{character}");
                    currPartNumber.EndIndex = i;
                }
            }
            else if (currPartNumber != null)
            {
                partNumbers.Add(currPartNumber);
                currPartNumber = null;
            }
        }

        if (currPartNumber != null)
        {
            partNumbers.Add(currPartNumber);
        }

        return partNumbers;
    }
    
    public static List<Symbol> FindSymbolsForRow(string row, int rowNumber)
    {
        var symbols = new List<Symbol>();
        
        for (int i=0; i < row.Length; i++)
        {
            var character = row[i];
            
            var isSymbol = !Char.IsLetterOrDigit(character);

            if (isSymbol && character.ToString() != ".")
            {
                var symbol = new Symbol()
                {
                    Value = character,
                    RowNumber = rowNumber,
                    Index = i
                };
                
                symbols.Add(symbol);
            }
        }

        return symbols;
    }
}
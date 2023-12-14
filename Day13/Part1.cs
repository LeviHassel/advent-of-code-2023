namespace AdventOfCode2023.Day13;

// https://adventofcode.com/2023/day/13

internal static class Part1
{
    public static void Run(string[] lines)
    {
        var patterns = new List<List<string>>();

        var currentPattern = new List<string>();
        
        foreach (var line in lines)
        {
            if (line == "")
            {
                patterns.Add(currentPattern);
                currentPattern = new List<string>();
                continue;
            }
            
            currentPattern.Add(line);
        }
        
        if (currentPattern.Any())
        {
            patterns.Add(currentPattern);
        }

        long patternSummary = 0;

        foreach (var patternRows in patterns)
        {
            var rowsBeforeReflection = new List<string>();
            var rowsAfterReflection = new List<string>();
            var foundDuplicateRow = false;
            var duplicateRowIndex = 0;
            var currentExpectedRowBeforeReflectionIndex = 0;

            // Search rows for reflection
            for (var i = 0; i < patternRows.Count(); i++)
            {
                // End early if we've found mirrored versions of all rows before the reflection
                if (foundDuplicateRow && rowsBeforeReflection.Count == rowsAfterReflection.Count || currentExpectedRowBeforeReflectionIndex < 0)
                {
                    break;
                }
                
                var row = patternRows[i];
                
                if (rowsBeforeReflection.Any() && rowsBeforeReflection.Last() == row)
                {
                    foundDuplicateRow = true;
                    duplicateRowIndex = i;
                    currentExpectedRowBeforeReflectionIndex = i - 2;
                    rowsAfterReflection.Add(row);
                    continue;
                }

                if (foundDuplicateRow)
                {
                    if (rowsBeforeReflection[currentExpectedRowBeforeReflectionIndex] == row)
                    {
                        rowsAfterReflection.Add(row);
                        currentExpectedRowBeforeReflectionIndex--;
                        continue;
                    }

                    // This wasn't the actual reflection, look for another
                    foundDuplicateRow = false;
                    currentExpectedRowBeforeReflectionIndex = 0;
                    rowsBeforeReflection.AddRange(rowsAfterReflection);
                    rowsBeforeReflection.Add(row);
                    rowsAfterReflection = new List<string>();
                    continue;
                }
                
                rowsBeforeReflection.Add(row);
            }

            if (foundDuplicateRow)
            {
                var rowsAboveReflection = duplicateRowIndex;
                patternSummary += (100 * rowsAboveReflection);
                continue;
            }
            
            var columnsBeforeReflection = new List<string>();
            var columnsAfterReflection = new List<string>();
            var foundDuplicateColumn = false;
            var duplicateColumnIndex = 0;
            var currentExpectedColumnBeforeReflectionIndex = 0;
            
            // Search columns for reflection
            for (var i = 0; i < patternRows.First().Length; i++)
            {
                // End early if we've found mirrored versions of all columns before the reflection
                if (foundDuplicateColumn && columnsBeforeReflection.Count == columnsAfterReflection.Count || currentExpectedColumnBeforeReflectionIndex < 0)
                {
                    break;
                }

                string column = string.Join("", patternRows.Select(r => r[i]));
                
                if (columnsBeforeReflection.Any() && columnsBeforeReflection.Last() == column)
                {
                    foundDuplicateColumn = true;
                    duplicateColumnIndex = i;
                    currentExpectedColumnBeforeReflectionIndex = i - 2;
                    columnsAfterReflection.Add(column);
                    continue;
                }

                if (foundDuplicateColumn)
                {
                    if (columnsBeforeReflection[currentExpectedColumnBeforeReflectionIndex] == column)
                    {
                        columnsAfterReflection.Add(column);
                        currentExpectedColumnBeforeReflectionIndex--;
                        continue;
                    }

                    // This wasn't the actual reflection, look for another
                    foundDuplicateColumn = false;
                    currentExpectedColumnBeforeReflectionIndex = 0;
                    columnsBeforeReflection.AddRange(columnsAfterReflection);
                    columnsBeforeReflection.Add(column);
                    columnsAfterReflection = new List<string>();
                    continue;
                }
                
                columnsBeforeReflection.Add(column);
            }

            if (foundDuplicateColumn)
            {
                var columnsLeftOfReflection = duplicateColumnIndex;
                patternSummary += columnsLeftOfReflection;
            }
        }
        
        Console.WriteLine($"Summary of patterns: {patternSummary}");
    }
}
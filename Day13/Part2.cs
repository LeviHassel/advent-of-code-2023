namespace AdventOfCode2023.Day13;

// https://adventofcode.com/2023/day/13

internal static class Part2
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
            var foundDuplicateRow = false;
            var duplicateRowIndex = 0;
            var rowSmudgeFound = false;
            var disqualifiedRowSmudgeIndexes = new List<int>();
            var disqualifiedReflectionIndexes = new List<int>();

            for (var rowAttempts=0; rowAttempts < 3; rowAttempts++)
            {
                var rowsBeforeReflection = new List<string>();
                var rowsAfterReflection = new List<string>();
                foundDuplicateRow = false;
                duplicateRowIndex = 0;
                rowSmudgeFound = false;
                int smudgeIndex = 0;

                var currentExpectedRowBeforeReflectionIndex = 0;
                
                // Search rows for reflection
                for (var i = 0; i < patternRows.Count(); i++)
                {
                    // End early if we've found mirrored versions of all rows before the reflection
                    if (foundDuplicateRow && rowsBeforeReflection.Count == rowsAfterReflection.Count || currentExpectedRowBeforeReflectionIndex < 0)
                    {
                        if (!rowSmudgeFound)
                        {
                            // This wasn't the actual reflection with a smudge, look for another
                            foundDuplicateRow = false;
                            if (!disqualifiedRowSmudgeIndexes.Contains(smudgeIndex) && smudgeIndex != 0)
                            {
                                disqualifiedRowSmudgeIndexes.Add(smudgeIndex);
                            }
                        
                            rowSmudgeFound = false;
                            smudgeIndex = 0;

                            currentExpectedRowBeforeReflectionIndex = 0;
                            rowsBeforeReflection.AddRange(rowsAfterReflection);
                            rowsAfterReflection = new List<string>();
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                    var row = patternRows[i];

                    var charCountDiffBetweenLastRowBeforeReflection = rowsBeforeReflection.Any() ? FindCharacterCountDifference(rowsBeforeReflection.Last(), row) : 100;

                    if (!foundDuplicateRow && !disqualifiedReflectionIndexes.Contains(i) && charCountDiffBetweenLastRowBeforeReflection <= 1)
                    {
                        if (charCountDiffBetweenLastRowBeforeReflection == 1)
                        {
                            if (!disqualifiedRowSmudgeIndexes.Contains(i))
                            {
                                rowSmudgeFound = true;
                                smudgeIndex = i;
                            }
                        }

                        if (charCountDiffBetweenLastRowBeforeReflection == 0 || !disqualifiedRowSmudgeIndexes.Contains(i))
                        {
                            foundDuplicateRow = true;
                            disqualifiedReflectionIndexes.Add(i);
                            duplicateRowIndex = i;
                            currentExpectedRowBeforeReflectionIndex = i - 2;
                            rowsAfterReflection.Add(row);
                            continue;
                        }
                    }

                    if (foundDuplicateRow)
                    {
                        var charCountDiff = FindCharacterCountDifference(rowsBeforeReflection[currentExpectedRowBeforeReflectionIndex], row);

                        if (charCountDiff == 1)
                        {
                            if (!rowSmudgeFound && !disqualifiedRowSmudgeIndexes.Contains(i))
                            {
                                rowSmudgeFound = true;
                                smudgeIndex = i;
                                rowsAfterReflection.Add(row);
                                currentExpectedRowBeforeReflectionIndex--;
                                continue;
                            }
                        }

                        if (charCountDiff == 0)
                        {
                            rowsAfterReflection.Add(row);
                            currentExpectedRowBeforeReflectionIndex--;
                            continue;
                        }

                        // This wasn't the actual reflection, look for another
                        foundDuplicateRow = false;
                        if (!disqualifiedRowSmudgeIndexes.Contains(smudgeIndex) && smudgeIndex != 0)
                        {
                            disqualifiedRowSmudgeIndexes.Add(smudgeIndex);
                        }
                        
                        rowSmudgeFound = false;
                        smudgeIndex = 0;

                        currentExpectedRowBeforeReflectionIndex = 0;
                        rowsBeforeReflection.AddRange(rowsAfterReflection);
                        rowsBeforeReflection.Add(row);
                        rowsAfterReflection = new List<string>();
                        continue;
                    }
                    
                    rowsBeforeReflection.Add(row);
                }

                if ((!foundDuplicateRow || !rowSmudgeFound) && smudgeIndex != 0)
                {
                    disqualifiedRowSmudgeIndexes.Add(smudgeIndex);
                }

                if (foundDuplicateRow && rowSmudgeFound)
                {
                    var rowsAboveReflection = duplicateRowIndex;
                    patternSummary += (100 * rowsAboveReflection);
                    break;
                }
            }

            if (foundDuplicateRow && rowSmudgeFound)
            {
                continue;
            }

            var foundDuplicateColumn = false;
            var duplicateColumnIndex = 0;
            var columnSmudgeFound = false;
            var disqualifiedColumnSmudgeIndexes = new List<int>();
            var disqualifiedColumnReflectionIndexes = new List<int>();
            
            for (var columnAttempts=0; columnAttempts < 3; columnAttempts++)
            { 
                var columnsBeforeReflection = new List<string>();
                var columnsAfterReflection = new List<string>();
                foundDuplicateColumn = false;
                duplicateColumnIndex = 0;
                columnSmudgeFound = false;
                int smudgeIndex = 0;

                var currentExpectedColumnBeforeReflectionIndex = 0;
                
                // Search columns for reflection
                for (var i = 0; i < patternRows.First().Count(); i++)
                {
                    // End early if we've found mirrored versions of all columns before the reflection
                    if (foundDuplicateColumn && columnsBeforeReflection.Count == columnsAfterReflection.Count || currentExpectedColumnBeforeReflectionIndex < 0)
                    {
                        if (!columnSmudgeFound)
                        {
                            // This wasn't the actual reflection with a smudge, look for another
                            foundDuplicateColumn = false;
                            if (!disqualifiedColumnSmudgeIndexes.Contains(smudgeIndex) && smudgeIndex != 0)
                            {
                                disqualifiedColumnSmudgeIndexes.Add(smudgeIndex);
                            }
                        
                            columnSmudgeFound = false;
                            smudgeIndex = 0;

                            currentExpectedColumnBeforeReflectionIndex = 0;
                            columnsBeforeReflection.AddRange(columnsAfterReflection);
                            columnsAfterReflection = new List<string>();
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                    string column = string.Join("", patternRows.Select(r => r[i]));

                    var charCountDiffBetweenLastColumnBeforeReflection = columnsBeforeReflection.Any() ? FindCharacterCountDifference(columnsBeforeReflection.Last(), column) : 100;

                    if (!foundDuplicateColumn && !disqualifiedColumnReflectionIndexes.Contains(i) && charCountDiffBetweenLastColumnBeforeReflection <= 1)
                    {
                        if (charCountDiffBetweenLastColumnBeforeReflection == 1)
                        {
                            if (!disqualifiedColumnSmudgeIndexes.Contains(i))
                            {
                                columnSmudgeFound = true;
                                smudgeIndex = i;
                            }
                        }

                        if (charCountDiffBetweenLastColumnBeforeReflection == 0 || !disqualifiedColumnSmudgeIndexes.Contains(i))
                        {
                            disqualifiedColumnReflectionIndexes.Add(i);
                            foundDuplicateColumn = true;
                            duplicateColumnIndex = i;
                            currentExpectedColumnBeforeReflectionIndex = i - 2;
                            columnsAfterReflection.Add(column);
                            continue;
                        }
                    }

                    if (foundDuplicateColumn)
                    {
                        var charCountDiff = FindCharacterCountDifference(columnsBeforeReflection[currentExpectedColumnBeforeReflectionIndex], column);

                        if (charCountDiff == 1)
                        {
                            if (!columnSmudgeFound && !disqualifiedColumnSmudgeIndexes.Contains(i))
                            {
                                columnSmudgeFound = true;
                                smudgeIndex = i;
                                columnsAfterReflection.Add(column);
                                currentExpectedColumnBeforeReflectionIndex--;
                                continue;
                            }
                        }

                        if (charCountDiff == 0)
                        {
                            columnsAfterReflection.Add(column);
                            currentExpectedColumnBeforeReflectionIndex--;
                            continue;
                        }

                        // This wasn't the actual reflection, look for another
                        foundDuplicateColumn = false;
                        if (!disqualifiedColumnSmudgeIndexes.Contains(smudgeIndex) && smudgeIndex != 0)
                        {
                            disqualifiedColumnSmudgeIndexes.Add(smudgeIndex);
                        }
                        
                        columnSmudgeFound = false;
                        smudgeIndex = 0;

                        currentExpectedColumnBeforeReflectionIndex = 0;
                        columnsBeforeReflection.AddRange(columnsAfterReflection);
                        columnsBeforeReflection.Add(column);
                        columnsAfterReflection = new List<string>();
                        continue;
                    }
                    
                    columnsBeforeReflection.Add(column);
                }

                if ((!foundDuplicateColumn || !columnSmudgeFound) && smudgeIndex != null)
                {
                    disqualifiedColumnSmudgeIndexes.Add(smudgeIndex);
                }

                if (foundDuplicateColumn && columnSmudgeFound)
                {
                    var columnsAboveReflection = duplicateColumnIndex;
                    patternSummary += columnsAboveReflection;
                    break;
                }
            }
        }
        
        Console.WriteLine($"Summary of patterns: {patternSummary}");
    }

    public static int FindCharacterCountDifference(string line1, string line2)
    {
        var diffCount = 0;
        
        for (var i = 0; i < line1.Length; i++)
        {
            if (line1[i] != line2[i])
            {
                diffCount++;
            }
        }

        return diffCount;
    }
}
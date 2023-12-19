namespace AdventOfCode2023.Day19;

// https://adventofcode.com/2023/day/19

internal static class Part2
{
    public static void Run(string[] lines)
    {
        var workflows = new Dictionary<string, List<Rule>>();
        
        // Identify workflows
        foreach (var line in lines)
        {
            if (line == "")
            {
                break;
            }

            var getWorkflowResponse = GetWorkflow(line);
            workflows.Add(getWorkflowResponse.Item1, getWorkflowResponse.Item2);
        }
        
        var startingPartDestination = new RuleDestination { Type = RuleDestinationType.Redirect, WorkflowRedirectName = "in" };
        var startingPart = new PartWithRanges()
        {
            MinX = 1,
            MaxX = 4000,
            MinM = 1,
            MaxM = 4000,
            MinA = 1,
            MaxA = 4000,
            MinS = 1,
            MaxS = 4000,
        };

        var acceptedPartRanges = new List<PartWithRanges>();
        var workflowQueue = new Queue<(PartWithRanges, RuleDestination)>();
        workflowQueue.Enqueue((startingPart, startingPartDestination));

        // Run through every possible outcome of the workflows
        while (workflowQueue.Any())
        {
            var (part, startingDestination) = workflowQueue.Dequeue();
            var rulesForWorkflow = workflows[startingDestination.WorkflowRedirectName];
            var workflowOutcomes = RunWorkflow(part, rulesForWorkflow);

            foreach (var workflowOutcome in workflowOutcomes)
            {
                var newPart = workflowOutcome.Item1;
                var newDestination = workflowOutcome.Item2;
                
                if (newDestination.Type is RuleDestinationType.Accept)
                {
                    acceptedPartRanges.Add(newPart);
                    continue;
                }
                
                if (newDestination.Type is RuleDestinationType.Redirect)
                {
                    workflowQueue.Enqueue((newPart, newDestination));
                }
            }
        }

        long totalCombinationsOfAcceptedRatings = acceptedPartRanges.Sum(p => p.Combinations);
        
        Console.WriteLine($"Total combinations of accepted ratings: {totalCombinationsOfAcceptedRatings}");
    }

    public static List<(PartWithRanges, RuleDestination)> RunWorkflow(PartWithRanges part, List<Rule> rules)
    {
        var workflowOutcomes = new List<(PartWithRanges, RuleDestination)>();
        var currentPart = GetDeepCopyOfPart(part);
        
        foreach (var rule in rules)
        {
            var minPartFieldValue = rule.ComparisonType == RuleComparisonType.None ? 0 : GetMinPartValueFromCharacter(currentPart, rule.FieldName);
            var maxPartFieldValue = rule.ComparisonType == RuleComparisonType.None ? 0 : GetMaxPartValueFromCharacter(currentPart, rule.FieldName);
            switch (rule.ComparisonType)
            {
                case RuleComparisonType.PartFieldGreaterThan:
                    // Continue if part does not contain passing range
                    if (maxPartFieldValue <= rule.ComparisonValue)
                    {
                        break;
                    }

                    // If this check is always true, return destination and stop workflow
                    if (minPartFieldValue > rule.ComparisonValue)
                    {
                        workflowOutcomes.Add((currentPart, rule.Destination));
                        return workflowOutcomes;
                    }
                    
                    // Otherwise find passing and failing ranges and continue workflow
                    var partPassingGreaterThanRule = GetDeepCopyOfPart(currentPart);
                    var partFailingGreaterThanRule = GetDeepCopyOfPart(currentPart);

                    UpdatePartMinField(partPassingGreaterThanRule, rule.FieldName, rule.ComparisonValue + 1);
                    UpdatePartMaxField(partFailingGreaterThanRule, rule.FieldName, rule.ComparisonValue);

                    workflowOutcomes.Add((partPassingGreaterThanRule, rule.Destination));
                    currentPart = partFailingGreaterThanRule;
                    
                    break;
                case RuleComparisonType.PartFieldLessThan:
                    // Continue if part does not contain passing range
                    if (minPartFieldValue >= rule.ComparisonValue)
                    {
                        break;
                    }

                    // If this check is always true, return destination and stop workflow
                    if (maxPartFieldValue < rule.ComparisonValue)
                    {
                        workflowOutcomes.Add((currentPart, rule.Destination));
                        return workflowOutcomes;
                    }
                    
                    // Otherwise find passing and failing ranges and continue workflow
                    var partPassingLessThanRule = GetDeepCopyOfPart(currentPart);
                    var partFailingLessThanRule = GetDeepCopyOfPart(currentPart);

                    UpdatePartMaxField(partPassingLessThanRule, rule.FieldName, rule.ComparisonValue - 1);
                    UpdatePartMinField(partFailingLessThanRule, rule.FieldName, rule.ComparisonValue);

                    workflowOutcomes.Add((partPassingLessThanRule, rule.Destination));
                    currentPart = partFailingLessThanRule;
                    
                    break;
                case RuleComparisonType.None:
                    workflowOutcomes.Add((currentPart, rule.Destination));
                    break;
            }
        }

        return workflowOutcomes;
    }

    public static PartWithRanges GetDeepCopyOfPart(PartWithRanges part)
    {
        return new PartWithRanges()
        {
            MinX = part.MinX,
            MaxX = part.MaxX,
            MinM = part.MinM,
            MaxM = part.MaxM,
            MinA = part.MinA,
            MaxA = part.MaxA,
            MinS = part.MinS,
            MaxS = part.MaxS,
        };
    }

    public static (string, List<Rule>) GetWorkflow(string line)
    {
        var lineContents = line.Split('{');
        var name = lineContents[0];
        var ruleStrings = lineContents[1].Substring(0, lineContents[1].Length - 1).Split(',');
        var rules = new List<Rule>();

        foreach (var ruleString in ruleStrings)
        {
            if (ruleString.Contains(':'))
            {
                var ruleContents = ruleString.Split(':');
                var comparison = ruleContents[0];
                var destinationString = ruleContents[1];
                var fieldName = comparison[0];
                var comparisonOperator = comparison[1];
                var value = int.Parse(comparison[2..]);
                
                rules.Add(new Rule
                {
                    FieldName = fieldName,
                    ComparisonValue = value,
                    ComparisonType = comparisonOperator == '>' ? RuleComparisonType.PartFieldGreaterThan : RuleComparisonType.PartFieldLessThan,
                    Destination = GetRuleDestination(destinationString)
                });
                
                continue;
            }
            
            rules.Add(new Rule
            {
                ComparisonType = RuleComparisonType.None,
                Destination = GetRuleDestination(ruleString)
            });
        }

        return (name, rules);
    }
    
    public static long GetMinPartValueFromCharacter(PartWithRanges part, char partFieldName)
    {
        return partFieldName switch
        {
            'x' => part.MinX,
            'm' => part.MinM,
            'a' => part.MinA,
            's' => part.MinS,
            _ => throw new ArgumentOutOfRangeException(nameof(partFieldName), partFieldName, null)
        };
    }
    
    public static long GetMaxPartValueFromCharacter(PartWithRanges part, char partFieldName)
    {
        return partFieldName switch
        {
            'x' => part.MaxX,
            'm' => part.MaxM,
            'a' => part.MaxA,
            's' => part.MaxS,
            _ => throw new ArgumentOutOfRangeException(nameof(partFieldName), partFieldName, null)
        };
    }
    
    public static void UpdatePartMinField(PartWithRanges part, char partFieldName, int newValue)
    {
        switch (partFieldName)
        {
            case 'x':
                part.MinX = newValue;
                break;
            case 'm':
                part.MinM = newValue;
                break;
            case 'a':
                part.MinA = newValue;
                break;
            case 's':
                part.MinS = newValue;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(partFieldName), partFieldName, null);
        }
    }
    
    public static void UpdatePartMaxField(PartWithRanges part, char partFieldName, int newValue)
    {
        switch (partFieldName)
        {
            case 'x':
                part.MaxX = newValue;
                break;
            case 'm':
                part.MaxM = newValue;
                break;
            case 'a':
                part.MaxA = newValue;
                break;
            case 's':
                part.MaxS = newValue;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(partFieldName), partFieldName, null);
        }
    }

    public static RuleDestination GetRuleDestination(string destinationName)
    {
        return destinationName switch
        {
            "A" => new RuleDestination { Type = RuleDestinationType.Accept },
            "R" => new RuleDestination { Type = RuleDestinationType.Reject },
            _ => new RuleDestination { Type = RuleDestinationType.Redirect, WorkflowRedirectName = destinationName },
        };
    }
}

internal class PartWithRanges
{
    public long MinX;
    public long MinM;
    public long MinA;
    public long MinS;
    public long MaxX;
    public long MaxM;
    public long MaxA;
    public long MaxS;
    public long Combinations => (MaxX - MinX + 1) * (MaxM - MinM + 1) * (MaxA - MinA + 1) * (MaxS - MinS + 1);
}
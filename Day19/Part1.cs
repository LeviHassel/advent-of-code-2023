namespace AdventOfCode2023.Day19;

// https://adventofcode.com/2023/day/19

internal static class Part1
{
    public static void Run(string[] lines)
    {
        var parts = new List<Part>();
        var workflows = new Dictionary<string, List<Rule>>();
        var lineContainsPart = false;
        
        // Identify workflows and parts
        foreach (var line in lines)
        {
            if (line == "")
            {
                lineContainsPart = true;
                continue;
            }

            if (lineContainsPart)
            {
                parts.Add(GetPart(line));
                continue;
            }

            var getWorkflowResponse = GetWorkflow(line);
            workflows.Add(getWorkflowResponse.Item1, getWorkflowResponse.Item2);
        }
        
        var partRatingsTotal = 0;

        var partDestination = new RuleDestination { Type = RuleDestinationType.Redirect, WorkflowRedirectName = "in" };

        // Run every part through their workflows
        foreach (var part in parts)
        {
            while (partDestination.Type is RuleDestinationType.Redirect)
            {
                var rulesForWorkflow = workflows[partDestination.WorkflowRedirectName];
                partDestination = RunWorkflow(part, rulesForWorkflow);
            }

            if (partDestination.Type is RuleDestinationType.Accept)
            {
                var partRating = part.RatingTotal;
                partRatingsTotal += partRating;
                Console.WriteLine($"Part with x={part.X} and total rating of {partRating} accepted");
            }

            partDestination = new RuleDestination { Type = RuleDestinationType.Redirect, WorkflowRedirectName = "in" };
        }
        
        Console.WriteLine($"Total of part ratings: {partRatingsTotal}");
    }

    public static RuleDestination RunWorkflow(Part part, List<Rule> rules)
    {
        foreach (var rule in rules)
        {
            var partFieldValue = rule.ComparisonType == RuleComparisonType.None ? 0 : GetPartValueFromCharacter(part, rule.FieldName);
            switch (rule.ComparisonType)
            {
                case RuleComparisonType.PartFieldGreaterThan:
                    if (partFieldValue > rule.ComparisonValue)
                    {
                        return rule.Destination;
                    }
                    break;
                case RuleComparisonType.PartFieldLessThan:
                    if (partFieldValue < rule.ComparisonValue)
                    {
                        return rule.Destination;
                    }
                    break;
                case RuleComparisonType.None:
                    return rule.Destination;
            }
        }

        throw new Exception("Invalid rule comparison type");
    }

    public static Part GetPart(string line)
    {
        var lineWithoutBrackets = line.Substring(1, line.Length - 2);

        var fields = lineWithoutBrackets.Split(',');

        var part = new Part();

        foreach (var field in fields)
        {
            var contents = field.Split('=');
            var fieldName = contents[0];
            var fieldValue = int.Parse(contents[1]);

            switch (fieldName)
            {
                case "x":
                    part.X = fieldValue;
                    break;
                case "m":
                    part.M = fieldValue;
                    break;
                case "a":
                    part.A = fieldValue;
                    break;
                case "s":
                    part.S = fieldValue;
                    break;
            }
        }

        return part;
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
    
    public static int GetPartValueFromCharacter(Part part, char partFieldName)
    {
        return partFieldName switch
        {
            'x' => part.X,
            'm' => part.M,
            'a' => part.A,
            's' => part.S,
            _ => throw new ArgumentOutOfRangeException(nameof(partFieldName), partFieldName, null)
        };
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

internal class Part
{
    public int X;
    public int M;
    public int A;
    public int S;
    public int RatingTotal => X + M + A + S;
}
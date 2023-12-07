namespace AdventOfCode2023.Day7;

// https://adventofcode.com/2023/day/7

internal static class Part2
{
    public static void Run(string[] lines)
    {
        var hands = new List<Hand>();
        
        foreach (var line in lines)
        {
            var contents = line.Split(' ');
            hands.Add(new Hand()
            {
                HandString = contents[0],
                Bid = int.Parse(contents[1])
            });
        }

        foreach (var hand in hands)
        {
            hand.Values = GetValuesFromHand(hand.HandString);
            hand.Type = GetTypeFromValues(hand.Values);
        }

        var handTypeGroups = hands.GroupBy(h => h.Type).OrderBy(gh => gh.Key).ToList();

        long totalWinnings = 0;
        long rank = 1;
        
        foreach (var handTypeGroup in handTypeGroups)
        {
            var orderedGroup = handTypeGroup
                .OrderBy(h => h.Values[0])
                .ThenBy(h => h.Values[1])
                .ThenBy(h => h.Values[2])
                .ThenBy(h => h.Values[3])
                .ThenBy(h => h.Values[4]);

            foreach (var hand in orderedGroup)
            {
                var handWinnings = hand.Bid * rank;
                totalWinnings += handWinnings;
                rank++;
            }
        }
        
        Console.WriteLine($"Total Winnings: {totalWinnings}");
    }

    public static List<int> GetValuesFromHand(string hand)
    {
        var values = new List<int>();
        
        // Possible Characters In Value Order: A, K, Q, T, 9, 8, 7, 6, 5, 4, 3, 2, J
        foreach (var character in hand)
        {
            if (Char.IsDigit(character))
            {
                values.Add(int.Parse(character.ToString()));
                continue;
            }

            switch (character)
            {
                case 'A':
                    values.Add(13);
                    break;
                case 'K':
                    values.Add(12);
                    break;
                case 'Q':
                    values.Add(11);
                    break;
                case 'T':
                    values.Add(10);
                    break;
                case 'J':
                    values.Add(1);
                    break;
            }
        }
        
        return values;
    }
    
    public static HandType GetTypeFromValues(List<int> values)
    {
        var valueGroups = values.GroupBy(v => v).ToList();

        var orderedGroups = valueGroups.OrderByDescending(vg => vg.Count()).ToList();
        var largestGroup = orderedGroups[0];
        var largestGroupCount = largestGroup.Count();
        var secondLargestGroupCount = largestGroupCount == 5 ? 0 : orderedGroups[1].Count();

        var jokerCountForLargestGroup = largestGroup.Key == 1 ? secondLargestGroupCount : values.Count(v => v == 1);

        if (jokerCountForLargestGroup + largestGroupCount == 5)
        {
            return HandType.FiveOfAKind;
        }
            
        if (jokerCountForLargestGroup + largestGroupCount == 4)
        {
            return HandType.FourOfAKind;
        }
        
        if (jokerCountForLargestGroup + largestGroupCount == 3)
        {
            if (secondLargestGroupCount == 2)
            {
                return HandType.FullHouse;
            }
            
            return HandType.ThreeOfAKind;
        }

        if (jokerCountForLargestGroup + largestGroupCount == 2)
        {
            if (secondLargestGroupCount == 2)
            {
                return HandType.TwoPair;
            }

            return HandType.OnePair;
        }
            
        return HandType.HighCard;
    }
}
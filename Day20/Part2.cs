namespace AdventOfCode2023.Day20;

// https://adventofcode.com/2023/day/20

internal static class Part2
{
    public static void Run(string[] lines)
    {
        var modules = GetModules(lines);
        
        var currentButtonPresses = 0;
        var rxTriggerModulesHaveReceivedThreeHighPulses = false;
        var rxParentModule = modules.Values.First(m => m.DestinationModules.Contains("rx"));
        var rxTriggerModules = modules.Values.Where(m => m.DestinationModules.Contains(rxParentModule.Name)).ToList();
        
        while (!rxTriggerModulesHaveReceivedThreeHighPulses)
        {
            var pulseQueue = new Queue<PulseMessage>();
            pulseQueue.Enqueue( new PulseMessage
            {
                Destination = "button",
                PulseType = PulseType.Low
            });
            currentButtonPresses++;
            
            while (pulseQueue.Any())
            {
                var pulseMessage = pulseQueue.Dequeue();

                var foundModule = modules.TryGetValue(pulseMessage.Destination, out var module);

                if (!foundModule)
                {
                    continue;
                }
                
                var pulseTypeToBroadcast = PulseType.Low;
                
                switch (module.Type)
                {
                    case ModuleType.Button:
                        pulseTypeToBroadcast = PulseType.Low;
                        break;
                    case ModuleType.Broadcaster:
                        pulseTypeToBroadcast = pulseMessage.PulseType;
                        break;
                    case ModuleType.FlipFlop:
                        if (pulseMessage.PulseType is PulseType.High)
                        {
                            continue;
                        }

                        if (module.FlipFlopOn)
                        {
                            module.FlipFlopOn = false;
                            pulseTypeToBroadcast = PulseType.Low;
                        }
                        else
                        {
                            module.FlipFlopOn = true;
                            pulseTypeToBroadcast = PulseType.High;
                        }
                        break;
                    case ModuleType.Conjunction:
                        module.ConjunctionLastPulseFromEachInput[pulseMessage.Source] = pulseMessage.PulseType;

                        if (module.ConjunctionLastPulseFromEachInput.All(i => i.Value is PulseType.High))
                        {
                            pulseTypeToBroadcast = PulseType.Low;
                        }
                        else
                        {
                            pulseTypeToBroadcast = PulseType.High;
                            module.ConjunctionHighPulseSentAtButtonIndexes.Add(currentButtonPresses);
                            Console.WriteLine($"Conjunction module \'{module.Name}\' sent out a high pulse after {currentButtonPresses} button presses");
                        }
                        break;
                }
                
                foreach (var destinationModuleName in module.DestinationModules)
                {
                    var pulseMessageToSend = new PulseMessage
                    {
                        Source = module.Name,
                        Destination = destinationModuleName,
                        PulseType = pulseTypeToBroadcast
                    };
                    
                    //Console.WriteLine($"{pulseMessageToSend.Source} -{pulseMessageToSend.PulseType.ToString().ToLower()}-> {pulseMessageToSend.Destination}");
                    
                    pulseQueue.Enqueue(pulseMessageToSend);
                }
            }
            
            rxTriggerModulesHaveReceivedThreeHighPulses = rxTriggerModules.All(m => m.ConjunctionHighPulseSentAtButtonIndexes.Count >= 3);
        }

        long minButtonPressesToSendLowPulseToRx = 1;

        foreach (var rxTriggerModule in rxTriggerModules)
        {
            var indexes = rxTriggerModule.ConjunctionHighPulseSentAtButtonIndexes;

            if (indexes[1] - indexes[0] == indexes[2] - indexes[1])
            {
                minButtonPressesToSendLowPulseToRx *= indexes[1] - indexes[0];
            }
            else
            {
                throw new Exception("Pattern does not repeat regularly");
            }
        }

        Console.WriteLine($"Minimum button presses required to deliver a single low pulse to the \'rx\' module: {minButtonPressesToSendLowPulseToRx}");
    }
    
    public static Dictionary<string, Module> GetModules(string[] lines)
    {
        var modules = new Dictionary<string, Module>();
        
        // Add button that starts the process
        modules.Add("button", new Module()
        {
            Name = "button",
            Type = ModuleType.Button,
            DestinationModules = new List<string> { "broadcaster" }
        });

        // Add modules for everything in text file
        foreach (var line in lines)
        {
            var contents = line.Split(" -> ");
            var input = contents[0];
            var output = contents[1];

            var module = new Module();

            if (input.StartsWith('&'))
            {
                module.Type = ModuleType.Conjunction;
                module.Name = input[1..];
                module.ConjunctionLastPulseFromEachInput = new Dictionary<string, PulseType>();
            }
            else if (input.StartsWith('%'))
            {
                module.Type = ModuleType.FlipFlop;
                module.Name = input[1..];
                module.FlipFlopOn = false;
            }
            else
            {
                module.Type = ModuleType.Broadcaster;
                module.Name = input;
            }

            module.DestinationModules = output.Split(", ").ToList();
            
            modules.Add(module.Name, module);
        }
        
        // Populate conjunction modules with inputs
        var conjunctionModules = modules.Where(m => m.Value.Type is ModuleType.Conjunction).ToList();
        
        foreach (var conjunctionModule in conjunctionModules)
        {
            var inputModules = modules.Where(m => m.Value.DestinationModules.Contains(conjunctionModule.Key));
            foreach (var inputModule in inputModules)
            {
                conjunctionModule.Value.ConjunctionLastPulseFromEachInput.Add(inputModule.Key, PulseType.Low);
            }
        }

        return modules;
    }
}
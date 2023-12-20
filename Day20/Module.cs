namespace AdventOfCode2023.Day20;

internal class Module
{
    public string Name;
    public ModuleType Type;
    public List<string> DestinationModules;
    public bool FlipFlopOn;
    public Dictionary<string, PulseType> ConjunctionLastPulseFromEachInput;
    public List<int> ConjunctionHighPulseSentAtButtonIndexes = new List<int>();
}
namespace ReadingTracker;

public class NotImplementedMenuAction(string description) : IMenuAction
{
    public string Description { get; } = description;

    public void Execute()
    {
        Console.WriteLine($"{Description} is not implemented yet.");
    }
}
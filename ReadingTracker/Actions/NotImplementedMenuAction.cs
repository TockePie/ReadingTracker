namespace ReadingTracker.Actions;

public class NotImplementedMenuAction(string description) : IMenuAction
{
    public string Description { get; } = description;

    public Task ExecuteAsync()
    {
        Console.WriteLine($"{Description} is not implemented yet.");

        return Task.CompletedTask;
    }
}

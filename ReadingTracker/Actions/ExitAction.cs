namespace ReadingTracker.Actions;

public class ExitAction : IMenuAction
{
    public string Description => "Exit";

    public Task ExecuteAsync()
    {
        Environment.Exit(0);

        return Task.CompletedTask;
    }
}

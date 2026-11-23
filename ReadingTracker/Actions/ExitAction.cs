namespace ReadingTracker.Actions;

public class ExitAction : IMenuAction
{
    public string Description => "Exit";

    public Task<bool> ExecuteAsync()
    {
        return Task.FromResult(false);
    }
}

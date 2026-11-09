namespace ReadingTracker.Actions;

public class ExitAction : IMenuAction
{
    public string Description => "Exit";

    public void Execute()
    {
        Environment.Exit(0);
    }
}

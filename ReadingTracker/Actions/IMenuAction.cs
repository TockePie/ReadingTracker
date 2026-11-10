namespace ReadingTracker.Actions;

public interface IMenuAction
{
    string Description { get; }
    Task ExecuteAsync();
}

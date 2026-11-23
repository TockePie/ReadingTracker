namespace ReadingTracker.Actions;

public interface IMenuAction
{
    string Description { get; }
    Task<bool> ExecuteAsync();
}

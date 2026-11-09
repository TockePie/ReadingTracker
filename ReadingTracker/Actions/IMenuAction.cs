namespace ReadingTracker.Actions;

public interface IMenuAction
{
    string Description { get; }
    void Execute();
}

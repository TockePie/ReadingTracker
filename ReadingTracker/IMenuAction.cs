namespace ReadingTracker;

public interface IMenuAction
{
    string Description { get; }
    void Execute();
}
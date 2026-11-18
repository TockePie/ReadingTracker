namespace ReadingTracker.Storage;

public interface ICsvStorage
{
    void Save<T>(IEnumerable<T> content) where T : IEntity;
}

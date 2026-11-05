namespace ReadingTracker.Storage;

public interface IStorage
{
    Task Save<T>(T content);
    Task<List<T>> Get<T>();
}

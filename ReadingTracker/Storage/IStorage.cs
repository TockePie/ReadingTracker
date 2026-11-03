namespace ReadingTracker.Storage;

public interface IStorage
{
    Task Save<T>(string path, T content);
}

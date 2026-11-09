namespace ReadingTracker.Storage;

public interface IStorage
{
    Task<List<T>> Get<T>();
    Task Save<T>(T content);
    Task RemoveById<T>(Guid id) where T : IEntity;
}

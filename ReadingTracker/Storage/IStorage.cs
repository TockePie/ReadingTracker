namespace ReadingTracker.Storage;

public interface IStorage
{
    Task<List<T>> Get<T>();
    Task Save<T>(T content);
    Task UpdateItem<T>(Guid id, T content) where T : IEntity;
    Task RemoveById<T>(Guid id) where T : IEntity;
}

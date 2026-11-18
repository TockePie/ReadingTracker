namespace ReadingTracker.Storage;

public interface IStorage
{
    Task<List<T>> Get<T>() where T : IEntity;
    Task Save<T>(T content) where T : IEntity;
    Task UpdateItem<T>(Guid id, T content) where T : IEntity;
    Task RemoveById<T>(Guid id) where T : IEntity;
}

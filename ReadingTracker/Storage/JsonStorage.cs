using System.Text.Json;

namespace ReadingTracker.Storage;

public class JsonStorage(string filePath) : IStorage
{
    private readonly string _filePath = filePath;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public async Task<List<T>> Get<T>() where T : IEntity
    {
        List<T> parsedList = [];

        if (File.Exists(_filePath))
        {
            using var readStream = File.OpenRead(_filePath);
            parsedList = await JsonSerializer.DeserializeAsync<List<T>>(readStream) ?? [];
        }

        return parsedList;
    }

    public async Task Save<T>(T content) where T : IEntity
    {
        var parsedList = await Get<T>();

        parsedList.Add(content);

        await WriteDataIntoFile(parsedList);
    }

    public async Task UpdateItem<T>(Guid id, T content) where T : IEntity
    {
        var parsedList = await Get<T>();

        var index = parsedList.FindIndex(item => item.Id == id);
        if (index == -1)
        {
            throw new InvalidOperationException("Item not found.");
        }

        parsedList[index] = content;

        await WriteDataIntoFile(parsedList);
    }

    public async Task RemoveById<T>(Guid id) where T : IEntity
    {
        var parsedList = await Get<T>();

        var removedItems = parsedList.RemoveAll(item => item.Id == id);
        if (removedItems == 0) return;

        await WriteDataIntoFile(parsedList);
    }

    private async Task WriteDataIntoFile<T>(T content)
    {
        using var writeStream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(writeStream, content, _jsonOptions);
    }
}

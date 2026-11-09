using System.Text.Json;

namespace ReadingTracker.Storage;

public class JsonStorage(string filePath) : IStorage
{
    private readonly string _filePath = filePath;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public async Task<List<T>> Get<T>()
    {
        List<T> parsedList = [];

        if (File.Exists(_filePath))
        {
            using var readStream = File.OpenRead(_filePath);
            parsedList = await JsonSerializer.DeserializeAsync<List<T>>(readStream) ?? [];
        }

        return parsedList;
    }

    public async Task Save<T>(T content)
    {
        var parsedList = await Get<T>();

        parsedList.Add(content);

        await WriteDataIntoFile(content);
    }

    public async Task RemoveById<T>(Guid id) where T : IEntity
    {
        var parsedList = await Get<T>();

        var filteredList = parsedList.Where(item => item.Id != id).ToList();

        await WriteDataIntoFile(filteredList);
    }

    private async Task WriteDataIntoFile<T>(T content)
    {
        using var writeStream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(writeStream, content, _jsonOptions);
    }
}
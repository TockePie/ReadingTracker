using System.Text.Json;

namespace ReadingTracker.Storage;

public class JsonStorage : IStorage
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public async Task Save<T>(string path, T content)
    {
        List<T> parsedList = [];

        if (File.Exists(path))
        {
            using var readStream = File.OpenRead(path);
            parsedList = await JsonSerializer.DeserializeAsync<List<T>>(readStream) ?? [];
        }

        parsedList.Add(content);

        using var writeStream = File.Create(path);
        await JsonSerializer.SerializeAsync(writeStream, parsedList, _jsonOptions);
    }
}
using System.Text.Json;

namespace ReadingTracker.Storage;

public class JsonStorage : IStorage
{
    private readonly string _filePath;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public JsonStorage(string filePath)
    {
        _filePath = filePath;

        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public async Task<List<T>> Get<T>() where T : IEntity
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        try
        {
            await using var readStream = File.OpenRead(_filePath);
            return await JsonSerializer.DeserializeAsync<List<T>>(readStream, _jsonOptions) ?? [];
        }
        catch (JsonException ex)
        {
            throw new StorageException(
                $"File '{_filePath}' is corrupted or contains invalid JSON.",
                ex
            );
        }
        catch (IOException ex)
        {
            throw new StorageException(
                $"Can't read '{_filePath}' file. It may be opened by another program"
                , ex
            );
        }
    }

    public async Task Save<T>(T content) where T : IEntity
    {
        var parsedList = await Get<T>();

        if (parsedList.Any(item => item.Id == content.Id))
        {
            throw new InvalidOperationException($"Item with id {content.Id} already exists.");
        }

        parsedList.Add(content);

        await WriteDataIntoFile(parsedList);
    }

    public async Task UpdateItem<T>(Guid id, T content) where T : IEntity
    {
        if (id != content.Id)
        {
            throw new ArgumentException(
                "Id of item you want to update does not match with given id"
            );
        }

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
        var guid = Guid.NewGuid();
        var tempFilePath = $"{_filePath}.{guid}.tmp";

        try
        {
            await using (var writeStream = File.Create(tempFilePath))
            {
                await JsonSerializer.SerializeAsync(writeStream, content, _jsonOptions);
                await writeStream.FlushAsync();
            }

            File.Move(tempFilePath, _filePath, overwrite: true);
        }
        catch (Exception ex) when (ex is IOException or JsonException)
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }

            throw new StorageException($"Failed to save data to '{_filePath}'.", ex);
        }
    }
}

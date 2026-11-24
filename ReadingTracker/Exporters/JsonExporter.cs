using System.Text.Json;

namespace ReadingTracker.Exporters;

public class JsonExporter : IExporter
{
    public string FormatName => "JSON";

    public string FileExtension => "json";

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public async Task ExportAsync<T>(IEnumerable<T> content, string filePath)
    {
        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, content, _jsonOptions);
    }
}

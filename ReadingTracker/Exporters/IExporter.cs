namespace ReadingTracker.Exporters;

public interface IExporter
{
    string FormatName { get; }
    string FileExtension { get; }

    Task ExportAsync<T>(IEnumerable<T> content, string filePath);
}
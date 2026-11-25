using ReadingTracker.ConsoleHelper;
using ReadingTracker.Exporters;
using ReadingTracker.Models;
using ReadingTracker.Prompter;
using ReadingTracker.Storage;

namespace ReadingTracker.Actions;

public class ExportLibrary(
    IPrompter prompter,
    IStorage storage,
    IConsoleHelper consoleHelper,
    IEnumerable<IExporter> exporters
) : IMenuAction
{
    private const string ExportDirectory = "exports";
    public string Description => "Export library";
    private readonly List<IExporter> _exporters = [.. exporters];

    public async Task<bool> ExecuteAsync()
    {
        consoleHelper.PrintHeader(Description);

        var books = await storage.Get<Book>();
        if (books is not { Count: > 0 })
        {
            Console.WriteLine("Your library is empty");

            consoleHelper.PressToContinue();
            return true;
        }

        var exporter = SelectExporter();
        if (exporter is null)
        {
            return true;
        }

        var filePath = BuildExportPath(exporter.FileExtension);

        Directory.CreateDirectory(ExportDirectory);
        await exporter.ExportAsync(books, filePath);

        Console.WriteLine($"\nExport completed successfully! Saved to: {filePath}");

        consoleHelper.PressToContinue();
        return true;
    }

    private IExporter? SelectExporter()
    {
        var menuOptions = string.Join(
            Environment.NewLine,
            _exporters.Select((exporter, index) => $"{index + 1}. {exporter.FormatName}")
        );

        var chosenFormat = prompter.AskInt(
            $"""
            Choose format:

            {menuOptions}
            0. Back

            Select format: 
            """,
            input => input >= 0 && input <= _exporters.Count
        );

        if (chosenFormat == 0)
        {
            return null;
        }
        else
        {
            return _exporters[chosenFormat - 1];
        }
    }

    private static string BuildExportPath(string extension)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
        return Path.Combine(ExportDirectory, $"library_{timestamp}.{extension}");
    }
}

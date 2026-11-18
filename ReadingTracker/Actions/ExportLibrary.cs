using ReadingTracker.ConsoleHelper;
using ReadingTracker.Models;
using ReadingTracker.Prompter;
using ReadingTracker.Storage;

namespace ReadingTracker.Actions;

public class ExportLibrary(
    IPrompter prompter,
    IStorage storage,
    ICsvStorage csvStorage,
    IConsoleHelper consoleHelper
) : IMenuAction
{
    public string Description => "Export library";

    public async Task ExecuteAsync()
    {
        consoleHelper.PrintHeader(Description);

        var chosenFormat = prompter.AskInt(
            """
            Choose format:

            1. JSON
            2. CSV
            0. Back

            Select format: 
            """,
            input => input is >= 0 and < 3
        );

        if (chosenFormat == 0)
        {
            return;
        }

        var allBooks = await storage.Get<Book>();
        if (allBooks is not { Count: > 0 })
        {
            Console.WriteLine("Your library is empty");

            consoleHelper.PressToContinue();
            return;
        }

        if (chosenFormat == 2)
        {
            csvStorage.Save(allBooks);
        }

        Console.WriteLine("\nExport completed successfully!");

        consoleHelper.PressToContinue();
    }
}
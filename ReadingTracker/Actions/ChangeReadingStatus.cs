using ReadingTracker.ConsoleHelper;
using ReadingTracker.Extensions;
using ReadingTracker.Models;
using ReadingTracker.Prompter;
using ReadingTracker.Storage;

namespace ReadingTracker.Actions;

public class ChangeReadingStatus(
    IPrompter prompter,
    IStorage storage,
    IConsoleHelper consoleHelper
) : IMenuAction
{
    public string Description => "Change reading status";

    public async Task ExecuteAsync()
    {
        consoleHelper.PrintHeader(Description);

        var allBooks = await storage.Get<Book>();
        if (allBooks is not { Count: > 0 })
        {
            Console.WriteLine("\nYour library is empty");
            return;
        }

        var isValid = consoleHelper.TryPromptBookDetails(allBooks, out var selectedBook);
        if (!isValid)
        {
            return;
        }

        Console.WriteLine($"\nBook: {selectedBook.Title}");
        Console.WriteLine($"Current status: {selectedBook.Status.GetDescription()}");

        //TODO: Finish the method
    }
}

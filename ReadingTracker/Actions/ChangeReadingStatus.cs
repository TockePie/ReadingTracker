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
            Console.WriteLine("Your library is empty");

            consoleHelper.PressToContinue();
            return;
        }

        var isValid = consoleHelper.TryPromptBookDetails(allBooks, out var selectedBook);
        if (!isValid)
        {
            return;
        }

        Console.WriteLine($"\nBook: {selectedBook.Title}");
        Console.WriteLine($"Current status: {selectedBook.Status.GetDescription()}");

        var selectedReadingStatus = prompter.AskInt(
            """

            Choose new status:
            1. Want to Read
            2. Reading
            3. Completed
            0. Cancel

            New status: 
            """,
            input => input is >= 0 and < 4
        );

        if (selectedReadingStatus == 0)
        {
            return;
        }

        var newStatus = (ReadingStatus)(selectedReadingStatus - 1);
        if (newStatus == selectedBook.Status)
        {
            Console.WriteLine("\nStatus is already set to this value. No changes made.");

            consoleHelper.PressToContinue();
            return;
        }

        var updatedBook = selectedBook with { Status = newStatus };
        await storage.UpdateItem(selectedBook.Id, updatedBook);

        Console.WriteLine("\nStatus changed successfully!");

        consoleHelper.PressToContinue();
    }
}

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
        await UpdateBookStatusAsync(selectedBook, newStatus);

        consoleHelper.PressToContinue();
    }

    private async Task UpdateBookStatusAsync(Book book, ReadingStatus newStatus)
    {
        if (newStatus == book.Status)
        {
            Console.WriteLine("\nStatus is already set to this value. No changes made.");
            return;
        }

        var updatedBook = book with { Status = newStatus };
        await storage.UpdateItem(book.Id, updatedBook);

        Console.WriteLine("\nStatus changed successfully!");
    }
}

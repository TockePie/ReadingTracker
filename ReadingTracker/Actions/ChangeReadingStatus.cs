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

        DisplayBooks(books);
        Console.WriteLine();

        if (!consoleHelper.TryPromptBookDetails(books, out var selectedBook))
        {
            return true;
        }

        Console.WriteLine($"\nBook: {selectedBook.Title}");
        Console.WriteLine($"Current status: {selectedBook.Status.GetDescription()}");

        var statusChoise = prompter.AskInt(
            """

            Choose new status:
            1. Want to Read
            2. Reading
            3. Completed
            0. Cancel

            New status: 
            """,
            input => input is >= 0 and <= 3
        );

        if (statusChoise == 0)
        {
            return true;
        }

        var newStatus = (ReadingStatus)(statusChoise - 1);
        if (newStatus == selectedBook.Status)
        {
            Console.WriteLine("\nStatus is already set to this value. No changes made.");

            consoleHelper.PressToContinue();
            return true;
        }

        var updatedBook = selectedBook with { Status = newStatus };

        if (newStatus == ReadingStatus.Completed
            && selectedBook.TotalPages is int totalPages
            && selectedBook.CurrentPage < totalPages
        )
        {
            var response = prompter.Ask(
                """
                Set progress to 100% (all pages read)?
                [Y]es | [N]o: 
                """
            );
            if (response.Equals("Y", StringComparison.OrdinalIgnoreCase)
                || response.Equals("Yes", StringComparison.OrdinalIgnoreCase)
            )
            {
                updatedBook = updatedBook with { CurrentPage = totalPages };
            }
        }

        await storage.UpdateItem(updatedBook.Id, updatedBook);

        Console.WriteLine("\nStatus changed successfully!");

        consoleHelper.PressToContinue();
        return true;
    }

    private void DisplayBooks(IReadOnlyList<Book> books)
    {
        foreach (var (index, book) in books.Index())
        {
            var displayNumber = index + 1;
            Console.WriteLine($"{displayNumber}. {book.Title} - {book.DisplayProgress}");
        }
    }
}

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

        DisplayBooks(allBooks);
        Console.WriteLine();

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

        var bookToBeUpdated = selectedBook;

        if (newStatus == ReadingStatus.Completed && selectedBook.TotalPages is not null)
        {
            var shallSetCurrentPage = prompter.Ask(
                """
                Set progress to 100% (all pages read)?
                [Y]es
                [N]o

                """
            );
            if (shallSetCurrentPage.Equals("Y", StringComparison.OrdinalIgnoreCase))
            {
                bookToBeUpdated = selectedBook with { CurrentPage = selectedBook.TotalPages.Value };
            }
        }

        await UpdateBookStatusAsync(bookToBeUpdated, newStatus);

        consoleHelper.PressToContinue();
    }

    private void DisplayBooks(IReadOnlyList<Book> books)
    {
        foreach (var (index, book) in books.Index())
        {
            var displayNumber = index + 1;
            Console.WriteLine($"{displayNumber}. {book.Title} - {book.DisplayProgress}");
        }
    }

    private async Task UpdateBookStatusAsync(Book book, ReadingStatus newStatus)
    {
        var updatedBook = book with { Status = newStatus };
        await storage.UpdateItem(book.Id, updatedBook);

        Console.WriteLine("\nStatus changed successfully!");
    }
}

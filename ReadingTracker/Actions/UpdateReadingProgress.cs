using ReadingTracker.ConsoleHelper;
using ReadingTracker.Models;
using ReadingTracker.Prompter;
using ReadingTracker.Storage;

namespace ReadingTracker.Actions;

public class UpdateReadingProgress(
    IPrompter prompter,
    IStorage storage,
    IConsoleHelper consoleHelper
) : IMenuAction
{
    public string Description => "Update reading progress";

    public async Task<bool> ExecuteAsync()
    {
        consoleHelper.PrintHeader(Description);

        var allBooks = await storage.Get<Book>();
        var readingBooks = allBooks
            .Where(book => book.Status == ReadingStatus.Reading)
            .ToList();

        if (readingBooks is not { Count: > 0 })
        {
            Console.WriteLine("Your library is empty");

            consoleHelper.PressToContinue();
            return false;
        }

        Console.WriteLine("Your books currently being read:\n");

        DisplayBooks(readingBooks);
        Console.WriteLine();

        var isValid = consoleHelper.TryPromptBookDetails(readingBooks, out var selectedBook);
        if (!isValid)
        {
            return false;
        }

        var maxPages = selectedBook.TotalPages ?? int.MaxValue;
        var enteredPage = prompter.AskInt(
            "Enter your current page: ",
            input => input >= 0 && input <= maxPages
        );
        await UpdateBookCurrentPageAsync(selectedBook, enteredPage);

        consoleHelper.PressToContinue();
        return false;
    }

    private void DisplayBooks(IReadOnlyList<Book> books)
    {
        foreach (var (index, book) in books.Index())
        {
            var displayNumber = index + 1;
            Console.WriteLine($"{displayNumber}. {book.Title} - {book.DisplayProgress}");
        }
    }

    private async Task UpdateBookCurrentPageAsync(Book book, int enteredPage)
    {
        if (enteredPage == book.CurrentPage)
        {
            Console.WriteLine("\nNo changes made.");
            return;
        }

        var updatedBook = book with { CurrentPage = enteredPage };
        await storage.UpdateItem(book.Id, updatedBook);

        Console.WriteLine("\nProgress updated successfully!");
        Console.WriteLine($"New progress: {updatedBook.DisplayProgress}\n");
    }
}

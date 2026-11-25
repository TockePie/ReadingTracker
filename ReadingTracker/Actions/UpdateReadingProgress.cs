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

        var books = await storage.Get<Book>();
        var booksInProgress = books
            .Where(book => book.Status == ReadingStatus.Reading)
            .ToList();

        if (booksInProgress is not { Count: > 0 })
        {
            Console.WriteLine("You have no books currently being read.");

            consoleHelper.PressToContinue();
            return true;
        }

        Console.WriteLine("Your books currently being read:\n");

        DisplayBooks(booksInProgress);
        Console.WriteLine();

        if (!consoleHelper.TryPromptBookDetails(booksInProgress, out var selectedBook))
        {
            return true;
        }

        var maxPage = selectedBook.TotalPages ?? int.MaxValue;
        var prompt = selectedBook.TotalPages is int totalPages
            ? $"Enter your current page (0-{totalPages}): "
            : "Enter your current page: ";

        var enteredPage = prompter.AskInt(
            prompt,
            page => page >= 0 && page <= maxPage
        );

        await UpdateBookCurrentPageAsync(selectedBook, enteredPage);

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

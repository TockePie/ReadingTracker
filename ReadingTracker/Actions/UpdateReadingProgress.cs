using ReadingTracker.Models;
using ReadingTracker.Prompter;
using ReadingTracker.Storage;

namespace ReadingTracker.Actions;

public class UpdateReadingProgress(IPrompter prompter, IStorage storage) : IMenuAction
{
    public string Description => "Update reading progress";

    public async Task ExecuteAsync()
    {
        Console.WriteLine("\n========== UPDATE READING PROGRESS ==========");

        var allBooks = await storage.Get<Book>();
        //TODO: Filter here books only with status "Reading"
        if (allBooks is not { Count: > 0 })
        {
            Console.WriteLine("\nYour library is empty");
            return;
        }

        Console.WriteLine("\nYour books currently being read:\n");

        for (int i = 0; i < allBooks.Count; i++)
        {
            var progressSuffix = allBooks[i].TotalPages > 0
                ? $"/{allBooks[i].TotalPages} pages ({allBooks[i].CompletionRate}%)"
                : " pages";

            Console.WriteLine($"{i + 1}. {allBooks[i].Title} - {allBooks[i].CurrentPage}{progressSuffix}");
        }

        var isValid = TryPromptBookDetails(allBooks, out var selectedBook);
        if (!isValid)
        {
            return;
        }

        var maxPages = selectedBook.TotalPages ?? int.MaxValue;
        var enteredPage = prompter.AskInt(
            "Enter your current page: ",
            input => input > 0 && input <= maxPages
        );

        var updatedBook = selectedBook with { CurrentPage = enteredPage };

        await storage.UpdateItem(selectedBook.Id, updatedBook);

        Console.WriteLine("\nProgress updated successfully!");

        var newProgressSuffix = updatedBook.TotalPages > 0
            ? $"/{updatedBook.TotalPages} ({updatedBook.CompletionRate}%)"
            : string.Empty;

        Console.WriteLine($"New progress: {updatedBook.CurrentPage}{newProgressSuffix}");
    }

    private bool TryPromptBookDetails(IReadOnlyList<Book> books, out Book? book)
    {
        var enteredNumber = prompter.AskInt(
            "\nSelect a book: ",
            isOptional: true
        );
        if (enteredNumber is not int displayNumber)
        {
            book = null;
            return false;
        }

        var bookIndex = displayNumber - 1;
        if (bookIndex < 0 || bookIndex >= books.Count)
        {
            Console.WriteLine($"\nNo book found with ID: {displayNumber}");

            book = null;
            return false;
        }

        book = books[bookIndex];
        return true;
    }
}

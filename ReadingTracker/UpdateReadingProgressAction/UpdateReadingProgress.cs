using ReadingTracker.BookAdderAction;
using ReadingTracker.Prompter;
using ReadingTracker.Storage;

namespace ReadingTracker.UpdateReadingProgressAction;

public class UpdateReadingProgress(IPrompter prompter, IStorage storage) : IMenuAction
{
    public void Execute()
    {
        Console.WriteLine("\n========== UPDATE READING PROGRESS ==========");

        var savedBooks = storage.Get<Book>().GetAwaiter().GetResult();
        if (savedBooks is not { Count: > 0 })
        {
            Console.WriteLine("\nYour library is empty");
            return;
        }

        Console.WriteLine("\nYour books currently being read:\n");

        for (int i = 0; i < savedBooks.Count; i++)
        {
            var progressSuffix = savedBooks[i].TotalPages > 0
                ? $"/{savedBooks[i].TotalPages} pages ({savedBooks[i].CompletionRate}%)"
                : " pages";

            Console.WriteLine($"{i + 1}. {savedBooks[i].Title} - {savedBooks[i].CurrentPage}{progressSuffix}");
        }

        var isValid = TryPromptBookDetails(savedBooks, out var selectedBook);
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

        storage.UpdateItem(selectedBook.Id, updatedBook).GetAwaiter().GetResult();

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
using ReadingTracker.BookAdderAction;
using ReadingTracker.Prompter;
using ReadingTracker.Storage;

namespace ReadingTracker.MyLibraryAction;

public class MyLibrary(IPrompter prompter, IStorage storage) : IMenuAction
{
    public void Execute()
    {
        Console.WriteLine("========== MY LIBRARY ==========");

        var savedBooks = storage.Get<Book>().GetAwaiter().GetResult();

        if (savedBooks is not { Count: > 0 })
        {
            Console.WriteLine("\nYour library is empty");
            return;
        }

        DisplayBooks(savedBooks);

        Console.WriteLine("\n--------------------------------");

        var selectedOption = prompter.AskInt(@"
1. View book details
2. Edit book information
3. Remove a book
4. Back to main menu",
            input => input is >= 1 and < 5
        );

        //TODO: Implement options above
    }

    private void DisplayBooks(List<Book> savedBooks)
    {
        for (int i = 0; i < savedBooks.Count; i++)
        {
            var book = savedBooks[i];

            var progressSuffix = book.TotalPages > 0
                ? $"/{book.TotalPages} ({book.CompletionRate}%)"
                : string.Empty;

            Console.WriteLine($"\nID: {i + 1} | {book.Title} - {book.Author}");
            Console.WriteLine($"\tStatus: {book.Status} | Progress: {book.CurrentPage}{progressSuffix}");
        }
    }
}
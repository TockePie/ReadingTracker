using ReadingTracker.BookAdderAction;
using ReadingTracker.Storage;

namespace ReadingTracker.MyLibraryAction;

public class MyLibrary(IStorage storage) : IMenuAction
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
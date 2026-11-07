using ReadingTracker.BookAdderAction;
using ReadingTracker.Prompter;
using ReadingTracker.Storage;

namespace ReadingTracker.MyLibraryAction;

public class MyLibrary(IPrompter prompter, IStorage storage) : IMenuAction
{
    public void Execute()
    {
        Console.WriteLine("\n========== MY LIBRARY ==========");

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
4. Back to main menu
",
            input => input is >= 1 and < 5
        );

        switch (selectedOption)
        {
            case 1:
                {
                    AskBookDetails(savedBooks);
                    break;
                }
            case 2:
            case 3:
                {
                    throw new NotImplementedException();
                }

            case 4:
                {
                    return;
                }
        }
    }

    private static void DisplayBooks(IReadOnlyList<Book> books)
    {
        for (int i = 0; i < books.Count; i++)
        {
            var book = books[i];

            var progressSuffix = book.TotalPages > 0
                ? $"/{book.TotalPages} ({book.CompletionRate}%)"
                : string.Empty;

            Console.WriteLine($"\nID: {i + 1} | {book.Title} - {book.Author}");
            Console.WriteLine($"\tStatus: {book.Status} | Progress: {book.CurrentPage}{progressSuffix}");
        }
    }

    private void AskBookDetails(IReadOnlyList<Book> books)
    {
        var selectedId = prompter.AskInt("\nEnter book's ID: ", isOptional: true);
        if (selectedId is not int bookId)
        {
            return;
        }

        var bookIndex = bookId - 1;
        if (bookIndex < 0 || bookIndex >= books.Count)
        {
            Console.WriteLine($"\nNo book found with ID: {bookId}");
            return;
        }

        var foundBook = books[bookIndex];

        //TODO: Make the output prettier
        Console.WriteLine(foundBook);
    }
}
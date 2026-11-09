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

        while (true)
        {
            Console.WriteLine("\n--------------------------------");
            Console.WriteLine(@"
1. View book details
2. Edit book information
3. Remove a book
4. Back to main menu
");

            var selectedOption = prompter.AskInt(
                "Select an option: ",
                input => input is >= 1 and < 5
            );

            switch (selectedOption)
            {
                case 1:
                    {
                        AskBookDetails(savedBooks);

                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey(intercept: true);
                        break;
                    }

                case 2 or 3:
                    {
                        throw new NotImplementedException();
                    }

                case 4:
                    {
                        return;
                    }
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
        var selectedId = prompter.AskInt("Enter book's ID (press Enter to cancel): ", isOptional: true);
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
        PrintBookDetails(foundBook);
    }

    private static void PrintBookDetails(Book book)
    {
        Console.WriteLine("\n========== BOOK DETAILS ==========");

        PrintField("Title", book.Title);
        PrintField("Author", book.Author);

        if (book.PublicationYear is not null)
        {
            PrintField("Publication year", book.PublicationYear);
        }

        PrintField("Status", book.Status);

        if (book.Rating is not null)
        {
            PrintField("Rating", book.Rating);
        }

        if (book.TotalPages > 0)
        {
            PrintField(
                "Pages",
                $"{book.CurrentPage} / {book.TotalPages} ({book.CompletionRate}%)"
            );
        }
        else
        {
            PrintField("Current page", book.CurrentPage);
        }

        Console.WriteLine("==================================");
    }

    private static void PrintField(string label, object? value)
    {
        Console.WriteLine($"{label + ":",-20} {value}");
    }
}
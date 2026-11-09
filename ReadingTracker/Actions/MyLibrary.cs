using ReadingTracker.Models;
using ReadingTracker.Prompter;
using ReadingTracker.Storage;

namespace ReadingTracker.Actions;

public class MyLibrary(IPrompter prompter, IStorage storage) : IMenuAction
{
    public string Description => "My library";

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

            var menuChoise = prompter.AskInt(
                "Select an option: ",
                input => input is >= 1 and < 5
            );

            switch (menuChoise)
            {
                case 1:
                    {
                        var isValid = TryPromptBookDetails(savedBooks, out var selectedBook);
                        if (isValid)
                        {
                            PrintBookDetails(selectedBook);
                        }

                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey(intercept: true);
                        break;
                    }

                //TODO: Implement editing book information
                case 2:
                    {
                        throw new NotImplementedException();
                    }

                case 3:
                    {
                        var isValid = TryPromptBookDetails(savedBooks, out var selectedBook);
                        if (isValid)
                        {
                            RemoveBook(selectedBook);
                        }
                        return;
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
            var displayNumber = i + 1;

            var progressSuffix = book.TotalPages > 0
                ? $"/{book.TotalPages} ({book.CompletionRate}%)"
                : string.Empty;

            Console.WriteLine($"\nID: {displayNumber} | {book.Title} - {book.Author}");
            Console.WriteLine($"\tStatus: {book.Status} | Progress: {book.CurrentPage}{progressSuffix}");
        }
    }

    private bool TryPromptBookDetails(IReadOnlyList<Book> books, out Book? book)
    {
        var enteredNumber = prompter.AskInt(
            "Enter book's ID (press Enter to cancel): ",
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

    //TODO: move this method to a dedicated class
    private static void PrintField(string label, object? value)
    {
        Console.WriteLine($"{label + ":",-20} {value}");
    }

    private void RemoveBook(Book book)
    {
        storage.RemoveById<Book>(book.Id).GetAwaiter().GetResult();

        Console.WriteLine("Book removed successfully.");
    }
}

using ReadingTracker.ConsoleHelper;
using ReadingTracker.Models;
using ReadingTracker.Prompter;
using ReadingTracker.Storage;

namespace ReadingTracker.Actions;

public class SearchAndFilterBooks(
    IPrompter prompter,
    IStorage storage,
    IConsoleHelper consoleHelper
) : IMenuAction
{
    public string Description => "Search and filter my books";

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

        var selectedOption = prompter.AskInt(
            """
            1. Search by title
            2. Search by author
            3. Filter by reading status
            4. Filter by rating
            5. Sort by title
            6. Sort by publication year
            7. Sort by reading progress
            0. Back

            Select an option: 
            """,
            input => input is >= 0 and < 8
        );

        if (selectedOption == 0)
        {
            return;
        }

        switch (selectedOption)
        {
            case 1:
                SearchByString(
                    allBooks,
                    promptMessage: "Enter a book title: ",
                    book => book.Title
                );
                break;

            case 2:
                SearchByString(
                    allBooks,
                    promptMessage: "Enter an author: ",
                    book => book.Author
                );
                break;

            case 3:
                FilterByStatus(allBooks);
                break;

            case 4:
                FilterByRating(allBooks);
                break;

            case 5:
                DisplayBooks(allBooks.OrderBy(book => book.Title).ToList());
                break;

            case 6:
                DisplayBooks(allBooks.OrderBy(book => book.PublicationYear).ToList());
                break;

            case 7:
                DisplayBooks(allBooks
                    .OrderBy(book => book.CompletionRate)
                    .ThenBy(book => book.CurrentPage)
                    .ToList()
                );
                break;
        }

        consoleHelper.PressToContinue();
    }

    private void SearchByString(
        IReadOnlyList<Book> books,
        string promptMessage,
        Func<Book, string> propertySelector
    )
    {
        var searchTerm = prompter.Ask(promptMessage);

        var matches = books.Where(book => propertySelector(book).Contains(
            searchTerm,
            StringComparison.OrdinalIgnoreCase
        )).ToList();

        DisplayBooks(matches);
    }

    private void FilterByStatus(IReadOnlyList<Book> books)
    {
        var readingStatus = prompter.AskInt(
            """

            Reading status:
            1. Want to Read
            2. Reading
            3. Completed
            Select status: 
            """,
            input => input is >= 1 and < 4
        );

        var matches = books
            .Where(book => book.Status == (ReadingStatus)(readingStatus - 1))
            .ToList();

        DisplayBooks(matches);
    }

    private void FilterByRating(IReadOnlyList<Book> books)
    {
        var enteredRating = prompter.AskInt(
            "Enter a rating number from 1 to 5",
            input => input is >= 0 and <= 5
        );

        var matches = books
            .Where(book => book.Rating == enteredRating)
            .ToList();

        DisplayBooks(matches);
    }

    private void DisplayBooks(IReadOnlyList<Book> books)
    {
        Console.WriteLine();

        if (books is not { Count: > 0 })
        {
            Console.WriteLine("No books found matching this criteria.\n");
            return;
        }

        foreach (var book in books)
        {
            Console.WriteLine($"- {book.Title} - {book.Author}");
            Console.WriteLine($"  Status: {book.Status} | Progress: {book.DisplayProgress}\n");
        }
    }
}

using ReadingTracker.ConsoleHelper;
using ReadingTracker.Models;
using ReadingTracker.Prompter;
using ReadingTracker.Storage;

namespace ReadingTracker.Actions;

public class BookAdder(
    IPrompter prompter,
    IStorage storage,
    IConsoleHelper consoleHelper
) : IMenuAction
{
    public string Description => "Add a book manually";

    public async Task<bool> ExecuteAsync()
    {
        consoleHelper.PrintHeader(Description);

        var title = prompter.Ask("Title: ");
        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Title should not be empty");

            consoleHelper.PressToContinue();
            return false;
        }

        var author = prompter.Ask("Author: ");
        if (string.IsNullOrWhiteSpace(author))
        {
            Console.WriteLine("Author should not be empty");

            consoleHelper.PressToContinue();
            return false;
        }

        var currentYear = DateTime.Now.Year;
        var publicationYear = prompter.AskOptionalInt(
            "Publication year: ",
            year => year > 0 && year <= currentYear
        );

        var totalPages = prompter.AskOptionalInt(
            "Total pages: ",
            pages => pages > 0
        );

        var statusChoise = prompter.AskInt(
            """
            Reading status:
            1. Want to Read
            2. Reading
            3. Completed
            Select status:

            """,
            input => input is >= 1 and <= 3
        );
        var readingStatus = (ReadingStatus)(statusChoise - 1);

        var rating = prompter.AskOptionalInt(
            "Your rating (1-5, or skip): ",
            input => input is >= 1 and <= 5
        );

        var currentPage = readingStatus == ReadingStatus.Completed
            ? (totalPages ?? 0)
            : 0;

        var newBook = new Book(
            title,
            author,
            publicationYear,
            totalPages,
            readingStatus,
            rating,
            currentPage
        );

        await storage.Save(newBook);

        Console.WriteLine("\nBook added successfully!");

        consoleHelper.PressToContinue();
        return false;
    }
}

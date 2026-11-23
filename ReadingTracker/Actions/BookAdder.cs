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
        if (string.IsNullOrEmpty(title))
        {
            Console.WriteLine("Title should not be empty");

            consoleHelper.PressToContinue();
            return false;
        }
        var author = prompter.Ask("Author: ");
        if (string.IsNullOrEmpty(author))
        {
            Console.WriteLine("Author should not be empty");

            consoleHelper.PressToContinue();
            return false;
        }

        var currentYear = DateTime.Now.Year;
        var publicationYear = prompter.AskInt(
            "Publication year: ",
            isOptional: true,
            input => input > 0 && input <= currentYear
        );

        var totalPages = prompter.AskInt(
            "Total pages: ",
            isOptional: true,
            input => input is > 0
        );

        var selectedReadingStatus = prompter.AskInt(
            """
            Reading status:
            1. Want to Read
            2. Reading
            3. Completed
            Select status:

            """,
            input => input is >= 1 and < 4
        );
        var readingStatus = selectedReadingStatus - 1;

        var rating = prompter.AskInt(
            "Your rating (1-5, or skip): ",
            isOptional: true,
            input => input is > 0 and <= 5
        );

        var currentPage = totalPages is not null &&
            (ReadingStatus)readingStatus == ReadingStatus.Completed
                ? totalPages
                : 0;

        var bookToBeAdded = new Book(
            title,
            author,
            publicationYear,
            totalPages,
            (ReadingStatus)readingStatus,
            rating,
            (int)currentPage
        );

        await storage.Save(bookToBeAdded);

        Console.WriteLine("\nBook added successfully!");

        consoleHelper.PressToContinue();
        return false;
    }
}

using ReadingTracker.Models;
using ReadingTracker.Prompter;
using ReadingTracker.Storage;

namespace ReadingTracker.Actions;

public class BookAdder(IPrompter prompter, IStorage storage) : IMenuAction
{
    public string Description => "Add a book manually";

    public async Task ExecuteAsync()
    {
        Console.WriteLine("========== ADD BOOK MANUALLY ==========");

        var title = prompter.Ask("Title: ");
        var author = prompter.Ask("Author: ");

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

        var selectedReadingStatus = prompter.AskInt(@"
Reading status:
1. Want to Read
2. Reading
3. Completed
Select status:",
            input => input is >= 1 and < 4
        );
        var readingStatus = selectedReadingStatus - 1;

        var rating = prompter.AskInt(
            "\nYour rating (0-5, or skip):",
            isOptional: true,
            input => input is >= 0 and <= 5
        );

        var bookToBeAdded = new Book(
            title,
            author,
            publicationYear,
            totalPages,
            (ReadingStatus)readingStatus,
            rating
        );

        await storage.Save(bookToBeAdded);

        Console.WriteLine("\nBook added successfully!");
    }
}

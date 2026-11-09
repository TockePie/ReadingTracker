using ReadingTracker.Prompter;
using ReadingTracker.Storage;

namespace ReadingTracker.BookAdderAction;

public class BookAdder(IPrompter prompter, IStorage storage) : IMenuAction
{
    private readonly IPrompter _prompter = prompter;
    private readonly IStorage _storage = storage;

    public void Execute()
    {
        Console.WriteLine("========== ADD BOOK MANUALLY ==========");

        var title = _prompter.Ask("Title: ");
        var author = _prompter.Ask("Author: ");
        var publicationYear = _prompter.AskInt(
            "Publication year: ",
            isOptional: true,
            input => input is > 0 and <= 2026
        );
        var totalPages = _prompter.AskInt(
            "Total pages: ",
            isOptional: true,
            input => input is > 0
        );

        var selectedReadingStatus = _prompter.AskInt(@"
Reading status:
1. Want to Read
2. Reading
3. Completed
Select status:",
            input => input is >= 1 and < 4
        ) - 1;

        var rating = _prompter.AskInt(
            "\nYour rating (0-5, or skip):",
            isOptional: true,
            input => input is >= 0 and <= 5
        );

        var bookToBeAdded = new Book(
            title,
            author,
            publicationYear,
            totalPages,
            (ReadingStatus)selectedReadingStatus,
            rating
        );

        _storage.Save(bookToBeAdded).GetAwaiter().GetResult();

        Console.WriteLine("\nBook added successfully!");
    }
}

using System.Text.Json;
using ReadingTracker.Prompter;

namespace ReadingTracker.BookAdder;

public class BookAdder(IPrompter prompter) : IMenuAction
{
    private readonly IPrompter _prompter = prompter;

    public void Execute()
    {
        Console.WriteLine("========== ADD BOOK MANUALLY ==========");

        var title = _prompter.Ask("Title:");
        var author = _prompter.Ask("Author:");
        var publicationYear = _prompter.AskInt(
            "Publication year:",
            isOptional: true,
            input => input is > 0 and <= 2026
        );
        var totalPages = _prompter.AskInt("Total pages:", isOptional: true);

        var selectedReadingStatus =
            _prompter.AskInt(
                @"
Reading status:
1. Want to Read
2. Reading
3. Completed
Select status:",
                input => input is >= 1 and < 4
            ) - 1;

        var rating = _prompter.AskInt(
            "\nYour rating (0-5, or skip): ",
            isOptional: true,
            input => input is >= 0 and <= 5
        );

        // XXX: The logic of saving it to a file should be present here

        Console.WriteLine("\nBook added successfully!");

        //XXX: For testing purposes. Should be refactored later
        var bookToBeAdded = new BookDto(
            title,
            author,
            publicationYear,
            totalPages,
            (ReadingStatus)selectedReadingStatus,
            rating
        );

        var json = JsonSerializer.Serialize(bookToBeAdded);
        Console.WriteLine(json);
    }
}

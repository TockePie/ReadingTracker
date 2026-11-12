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

        switch (selectedOption)
        {
            case 1:
                {
                    var enteredTitle = prompter.Ask("Enter a book title: ");
                    var titles = allBooks.Where(book => book.Title.Contains(
                        enteredTitle,
                        StringComparison.OrdinalIgnoreCase
                    ));
                    //TODO: Display titles

                    break;
                }
        }
    }
}
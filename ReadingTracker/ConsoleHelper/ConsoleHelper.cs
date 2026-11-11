using ReadingTracker.Models;
using ReadingTracker.Prompter;

namespace ReadingTracker.ConsoleHelper;

public class ConsoleHelper(IPrompter prompter) : IConsoleHelper
{
    public void PrintHeader(string label)
    {
        Console.WriteLine($"\n========== {label.ToUpper()} ==========\n");
    }

    public void PrintField(string label, object? value)
    {
        Console.WriteLine($"{label + ":",-20} {value}");
    }

    public bool TryPromptBookDetails(IReadOnlyList<Book> books, out Book? book)
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
}
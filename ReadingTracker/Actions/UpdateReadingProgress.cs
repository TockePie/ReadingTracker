using ReadingTracker.ConsoleHelper;
using ReadingTracker.Models;
using ReadingTracker.Prompter;
using ReadingTracker.Storage;

namespace ReadingTracker.Actions;

public class UpdateReadingProgress(
    IPrompter prompter,
    IStorage storage,
    IConsoleHelper consoleHelper
) : IMenuAction
{
    public string Description => "Update reading progress";

    public async Task ExecuteAsync()
    {
        Console.WriteLine("\n========== UPDATE READING PROGRESS ==========");

        var allBooks = await storage.Get<Book>();
        //TODO: Filter here books only with status "Reading"
        if (allBooks is not { Count: > 0 })
        {
            Console.WriteLine("\nYour library is empty");
            return;
        }

        Console.WriteLine("\nYour books currently being read:\n");

        for (int i = 0; i < allBooks.Count; i++)
        {
            var book = allBooks[i];
            var displayNumber = i + 1;

            Console.WriteLine($"{displayNumber}. {book.Title} - {book.DisplayProgress}");
        }

        var isValid = consoleHelper.TryPromptBookDetails(allBooks, out var selectedBook);
        if (!isValid)
        {
            return;
        }

        var maxPages = selectedBook.TotalPages ?? int.MaxValue;
        var enteredPage = prompter.AskInt(
            "Enter your current page: ",
            input => input > 0 && input <= maxPages
        );

        var updatedBook = selectedBook with { CurrentPage = enteredPage };

        await storage.UpdateItem(selectedBook.Id, updatedBook);

        Console.WriteLine("\nProgress updated successfully!");
        Console.WriteLine($"New progress: {updatedBook.DisplayProgress}");
    }
}

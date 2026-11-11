using ReadingTracker.ConsoleHelper;
using ReadingTracker.Models;
using ReadingTracker.Prompter;
using ReadingTracker.Storage;

namespace ReadingTracker.Actions;

public class MyLibrary(
    IPrompter prompter,
    IStorage storage,
    IConsoleHelper consoleHelper
) : IMenuAction
{
    public string Description => "My library";

    public async Task ExecuteAsync()
    {
        consoleHelper.PrintHeader(Description);

        var savedBooks = await storage.Get<Book>();
        if (savedBooks is not { Count: > 0 })
        {
            Console.WriteLine("Your library is empty");
            return;
        }

        DisplayBooks(savedBooks);

        while (true)
        {
            Console.WriteLine("--------------------------------");
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
                        var isValid = consoleHelper.TryPromptBookDetails(savedBooks, out var selectedBook);
                        if (isValid)
                        {
                            PrintBookDetails(selectedBook);
                        }

                        Console.WriteLine("\nPress any key to continue...\n");
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
                        var isValid = consoleHelper.TryPromptBookDetails(savedBooks, out var selectedBook);
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

            Console.WriteLine($"ID: {displayNumber} | {book.Title} - {book.Author}");
            Console.WriteLine($"\tStatus: {book.Status} | Progress: {book.DisplayProgress}\n");
        }
    }

    private void PrintBookDetails(Book book)
    {
        Console.WriteLine("\n========== BOOK DETAILS ==========");

        consoleHelper.PrintField("Title", book.Title);
        consoleHelper.PrintField("Author", book.Author);

        if (book.PublicationYear is not null)
        {
            consoleHelper.PrintField("Publication year", book.PublicationYear);
        }

        consoleHelper.PrintField("Status", book.Status);

        if (book.Rating is not null)
        {
            consoleHelper.PrintField("Rating", book.Rating);
        }

        if (book.TotalPages > 0)
        {
            consoleHelper.PrintField(
                "Pages",
                $"{book.CurrentPage} / {book.TotalPages} ({book.CompletionRate}%)"
            );
        }
        else
        {
            consoleHelper.PrintField("Current page", book.CurrentPage);
        }

        Console.WriteLine("==================================");
    }

    private async Task RemoveBook(Book book)
    {
        await storage.RemoveById<Book>(book.Id);

        Console.WriteLine("Book removed successfully.");
    }
}

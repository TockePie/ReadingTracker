using ReadingTracker.ConsoleHelper;
using ReadingTracker.Extensions;
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

    public async Task<bool> ExecuteAsync()
    {
        consoleHelper.PrintHeader(Description);

        var allBooks = await storage.Get<Book>();
        if (allBooks is not { Count: > 0 })
        {
            Console.WriteLine("Your library is empty");

            consoleHelper.PressToContinue();
            return true;
        }

        DisplayBooks(allBooks);

        while (true)
        {
            Console.WriteLine(
                """
                --------------------------------
                
                1. View book details
                2. Edit book information
                3. Remove a book
                4. Back to main menu

                """
            );

            var menuChoice = prompter.AskInt(
                "Select an option: ",
                input => input is >= 1 and < 5
            );

            switch (menuChoice)
            {
                case 1:
                    {
                        var isValid = consoleHelper.TryPromptBookDetails(allBooks, out var selectedBook);
                        if (!isValid) break;

                        PrintBookDetails(selectedBook);

                        consoleHelper.PressToContinue();
                        break;
                    }

                case 2:
                    {
                        var isValid = consoleHelper.TryPromptBookDetails(allBooks, out var selectedBook);
                        if (!isValid) break;

                        await EditBookAsync(selectedBook);

                        consoleHelper.PressToContinue();
                        return true;
                    }

                case 3:
                    {
                        var isValid = consoleHelper.TryPromptBookDetails(allBooks, out var selectedBook);
                        if (!isValid) break;

                        await RemoveBook(selectedBook);

                        consoleHelper.PressToContinue();
                        return true;
                    }

                case 4:
                    Console.WriteLine();
                    return true;
            }
        }
    }

    private static void DisplayBooks(IReadOnlyList<Book> books)
    {
        foreach (var (index, book) in books.Index())
        {
            var displayNumber = index + 1;

            Console.WriteLine($"ID: {displayNumber} | {book.Title} - {book.Author}");
            Console.WriteLine($"\tStatus: {book.Status.GetDescription()} | Progress: {book.DisplayProgress}\n");
        }
    }

    private async Task EditBookAsync(Book book)
    {
        var currentYear = DateTime.Now.Year;

        var updatedBook = book with
        {
            Title = prompter.Ask("New title", book.Title),
            Author = prompter.Ask("New author", book.Author),
            PublicationYear = prompter.AskOptionalInt(
                "New publication year",
                defaultValue: book.PublicationYear,
                input => input > 0 && input <= currentYear
            ),
            TotalPages = prompter.AskOptionalInt(
                "New total pages",
                defaultValue: book.TotalPages,
                input => input > 0 && input >= book.CurrentPage,
                "The number of total pages should be at least as many as pages you have already read."
            ),
            Rating = prompter.AskOptionalInt(
                "New rating",
                defaultValue: book.Rating,
                input => input is > 0 and <= 5
            )
        };

        await storage.UpdateItem(book.Id, updatedBook);
        Console.WriteLine("\nBook updated successfully!");
    }

    private void PrintBookDetails(Book book)
    {
        Console.WriteLine("\n========== BOOK DETAILS ==========");

        consoleHelper.PrintField(
            "Title",
            book.Title
        );

        consoleHelper.PrintField(
            "Author",
            book.Author
        );

        if (book.PublicationYear is not null)
        {
            consoleHelper.PrintField(
                "Publication year",
                book.PublicationYear
            );
        }

        consoleHelper.PrintField(
            "Status",
            book.Status
        );

        if (book.Rating is not null)
        {
            consoleHelper.PrintField(
                "Rating",
                book.Rating
            );
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
            consoleHelper.PrintField(
                "Current page",
                book.CurrentPage
            );
        }

        Console.WriteLine("==================================\n");
    }

    private async Task RemoveBook(Book book)
    {
        await storage.RemoveById<Book>(book.Id);
        Console.WriteLine("Book removed successfully.");
    }
}

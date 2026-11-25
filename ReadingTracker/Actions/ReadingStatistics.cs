using ReadingTracker.ConsoleHelper;
using ReadingTracker.Models;
using ReadingTracker.Storage;

namespace ReadingTracker.Actions;

public class ReadingStatistics(
    IStorage storage,
    IConsoleHelper consoleHelper
) : IMenuAction
{
    public string Description => "Reading statistics";

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

        PrintStatistics(allBooks);

        Console.WriteLine();
        consoleHelper.PressToContinue();
        return true;
    }

    private void PrintStatistics(IReadOnlyList<Book> books)
    {
        consoleHelper.PrintField(
            "Total books",
            books.Count
        );

        consoleHelper.PrintField(
            "Want to Read",
            books.Count(book => book.Status == ReadingStatus.WantToRead)
        );

        consoleHelper.PrintField(
            "Currently reading",
            books.Count(book => book.Status == ReadingStatus.Reading)
        );

        consoleHelper.PrintField(
            "Completed",
            books.Count(book => book.Status == ReadingStatus.Completed)
        );
        Console.WriteLine();

        consoleHelper.PrintField(
            "Total pages read",
            books.Sum(book => book.CurrentPage)
        );

        consoleHelper.PrintField(
            "Pages remaining in library",
            books.Sum(book => Math.Max(0, (book.TotalPages - book.CurrentPage) ?? 0))
        );

        var averageRating = CalculateAverageRating(books);
        if (averageRating is not null)
        {
            consoleHelper.PrintField(
                "Average rating",
                averageRating
            );
        }
        Console.WriteLine();

        Console.WriteLine("Author with most completed books:");
        var (Author, Count) = CalculateMostReadAuthor(books);
        Console.WriteLine($"{Author} - {Count} books completed");
    }

    private (string, int) CalculateMostReadAuthor(IReadOnlyList<Book> books)
    {
        var mostAuthor = books
            .Where(book => book.Status == ReadingStatus.Completed)
            .GroupBy(book => book.Author)
            .Select(group => new { Author = group.Key, Count = group.Count() })
            .OrderByDescending(x => x.Count)
            .FirstOrDefault();

        if (mostAuthor is not null)
        {
            return (mostAuthor.Author, mostAuthor.Count);
        }

        return ("Unknown", 0);
    }

    private decimal? CalculateAverageRating(IReadOnlyList<Book> books)
    {
        var average = books.Average(book => book.Rating);

        if (average is not null)
        {
            return Math.Round((decimal)average, 1);
        }

        return null;
    }
}

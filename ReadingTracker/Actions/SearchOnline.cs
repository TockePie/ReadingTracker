using System.Net.Http.Json;
using System.Text.Json;
using System.Web;

using ReadingTracker.ConsoleHelper;
using ReadingTracker.Models;
using ReadingTracker.Prompter;
using ReadingTracker.Storage;

namespace ReadingTracker.Actions;

public class SearchOnline(
    HttpClient httpClient,
    IPrompter prompter,
    IStorage storage,
    IConsoleHelper consoleHelper,
    string baseUrl = "https://openlibrary.org/search.json"
) : IMenuAction
{
    public string Description => "Search books online";
    private const int PageSize = 10;

    public async Task ExecuteAsync()
    {
        consoleHelper.PrintHeader(Description);

        var userPrompt = prompter.Ask("Enter book title or author: ");
        if (string.IsNullOrWhiteSpace(userPrompt))
        {
            Console.WriteLine("Search query cannot be empty.");
            return;
        }

        var storageBooks = await storage.Get<Book>();
        int currentPage = 1;

        while (true)
        {
            Console.WriteLine($"Looking for books (Page: {currentPage})...\n");

            var books = await TryFetchPageAsync(userPrompt, currentPage);
            if (books is null) return;
            if (books.Count == 0)
            {
                Console.WriteLine("No books found on this page.");

                consoleHelper.PressToContinue();
                return;
            }

            DisplayBooks(books);
            PrintPaginationOptions(books.Count, currentPage);

            var selectedOption = prompter.Ask(
                "Select an option: "
            ).ToUpperInvariant();

            switch (selectedOption)
            {
                case "" or "C":
                    Console.WriteLine("Selection cancelled.");
                    return;

                case "N":
                    currentPage++;
                    continue;

                case "P" when currentPage > 1:
                    currentPage--;
                    continue;

                case var _ when int.TryParse(selectedOption, out int selectedIndex) &&
                            selectedIndex >= 1 &&
                            selectedIndex <= books.Count:
                    var selectedBook = books[selectedIndex - 1];
                    var isDuplicate = CheckIfBookIsInLibrary(
                        storageBooks,
                        selectedBook.Title,
                        FormatAuthors(selectedBook.AuthorName),
                        selectedBook.OpenLibraryKey
                    );
                    if (isDuplicate)
                    {
                        Console.WriteLine("You already have this book in your library.");
                    }
                    else
                    {
                        await HandleBookSelectionAsync(selectedBook);
                    }

                    consoleHelper.PressToContinue();
                    return;

                default:
                    Console.WriteLine("Invalid input. Please try again.");

                    consoleHelper.PressToContinue();
                    break;
            }
        }
    }

    private async Task<OpenLibrarySearchResponse> GetBooksAsync(
        string query,
        int page = 1,
        int limit = 10
    )
    {
        var queryParams = HttpUtility.ParseQueryString(string.Empty);
        queryParams["q"] = query;
        queryParams["page"] = page.ToString();
        queryParams["limit"] = limit.ToString();

        var uriBuilder = new UriBuilder(baseUrl)
        {
            Query = queryParams.ToString()
        };

        using var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
        request.Headers.UserAgent.ParseAdd(
            "ReadingTracker/1.0 (144056146+TockePie@users.noreply.github.com)"
        );

        using var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<OpenLibrarySearchResponse>();
    }

    private async Task<IReadOnlyList<ExternalBook>?> TryFetchPageAsync(string query, int page)
    {
        try
        {
            var response = await GetBooksAsync(
                query,
                page,
                limit: PageSize
            );
            return response?.Docs ?? [];
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            var message = ex switch
            {
                HttpRequestException { StatusCode: { } code } =>
                    $"External library returned an error (HTTP {(int)code}).",
                HttpRequestException =>
                    "This app can't connect to the external library. Check your internet connection.",
                TaskCanceledException =>
                    "The request to the external library timed out.",
                JsonException =>
                    "Received an invalid response from the external library.",
                _ => "An unexpected error occurred."
            };

            Console.WriteLine($"""
                Unable to search books :(
                {message}
                Please try again later.
                """
            );

            consoleHelper.PressToContinue();
            return null;
        }
    }

    private void DisplayBooks(IReadOnlyList<ExternalBook> books)
    {
        foreach (var (index, book) in books.Index())
        {
            var subtitle = book.Subtitle is not null
                ? $": {book.Subtitle}"
                : string.Empty;

            Console.WriteLine(
                $"""
                {index + 1}.  {book.Title}{subtitle}
                    Author: {FormatAuthors(book.AuthorName)}
                    First published: {book.FirstPublishYear}
                    Open library ID: {book.OpenLibraryKey}

                """
            );
        }
    }

    private void PrintPaginationOptions(int bookCount, int currentPage)
    {
        List<string> userOptions = [];
        userOptions.Add($"1-{bookCount}: Select book");
        userOptions.Add("[N]ext page");

        if (currentPage > 1)
        {
            userOptions.Add("[P]revious page");
        }

        userOptions.Add("[C]ancel");

        Console.WriteLine(string.Join(Environment.NewLine, userOptions));
        Console.WriteLine();
    }

    private bool CheckIfBookIsInLibrary(
        IReadOnlyList<Book> storageBooks,
        string title,
        string author,
        string? externalId
    )
    {
        return storageBooks.Any(book =>
            (!string.IsNullOrWhiteSpace(externalId) &&
             string.Equals(book.ExternalId, externalId, StringComparison.OrdinalIgnoreCase)
            )
            ||
            (string.Equals(book.Title.Trim(), title.Trim(), StringComparison.OrdinalIgnoreCase) &&
             string.Equals(book.Author.Trim(), author.Trim(), StringComparison.OrdinalIgnoreCase)
            )
        );
    }

    private async Task HandleBookSelectionAsync(ExternalBook book)
    {
        PrintBookDetails(book);

        var shouldBeAdded = prompter.Ask(
            """
            Would you like to add this book to your library?
            [Y]es
            [N]o
            
            """
        );

        if (!shouldBeAdded.Equals("Y", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

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

        var bookToBeAdded = new Book(
            book.Title,
            FormatAuthors(book.AuthorName),
            book.FirstPublishYear,
            totalPages,
            (ReadingStatus)readingStatus,
            rating
        )
        {
            ExternalId = book.OpenLibraryKey
        };

        await storage.Save(bookToBeAdded);

        Console.WriteLine("\nBook added successfully!");
    }

    private void PrintBookDetails(ExternalBook book)
    {
        Console.WriteLine("\n========== BOOK DETAILS ==========");

        consoleHelper.PrintField(
            "Title",
            book.Title ?? "Unknown Title"
        );

        consoleHelper.PrintField(
            "Author(s)",
            FormatAuthors(book.AuthorName)
        );

        if (book.FirstPublishYear is not null)
        {
            consoleHelper.PrintField(
                "First published",
                book.FirstPublishYear
            );
        }

        if (book.Language is { Count: > 0 })
        {
            consoleHelper.PrintField(
                "Languages",
                string.Join(", ", book.Language.Take(5))
            );
        }

        Console.WriteLine("==================================\n");
    }

    private static string FormatAuthors(IReadOnlyList<string>? authors)
    {
        return authors is { Count: > 0 }
            ? string.Join(", ", authors)
            : "Unknown";
    }
}
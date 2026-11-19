using System.Net.Http.Json;
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
    private readonly string _baseUrl = baseUrl;

    public async Task ExecuteAsync()
    {
        consoleHelper.PrintHeader(Description);

        var userPrompt = prompter.Ask("Enter book title or author: ");
        //TODO: Move hardcoded values out of the class
        int currentPage = 1;
        const int pageSize = 10;

        while (true)
        {
            Console.WriteLine($"Looking for books (Page: {currentPage})...\n");

            var books = await GetBooksAsync(
                userPrompt,
                page: currentPage,
                limit: pageSize
            );

            if (books?.Docs is null || books.Docs.Count == 0)
            {
                Console.WriteLine("No books found on this page.");

                consoleHelper.PressToContinue();
                break;
            }

            DisplayBooks(books.Docs);

            List<string> userOptions = [$"1-{books.Docs.Count}: Select book", "[N]ext page"];

            if (currentPage > 1)
            {
                userOptions.Add("[P]revious page");
            }

            userOptions.Add("[C]ancel");

            Console.WriteLine(string.Join("\n", userOptions));
            Console.WriteLine();

            var selectedOption = prompter.Ask(
                "Select an option: "
            ).ToUpperInvariant();

            if (selectedOption == "C" || string.IsNullOrEmpty(selectedOption))
            {
                Console.WriteLine("Selection cancelled.");
                return;
            }
            else if (selectedOption == "N")
            {
                currentPage++;
                continue;
            }
            else if (selectedOption == "P" && currentPage > 1)
            {
                currentPage--;
                continue;
            }
            else if (
                int.TryParse(selectedOption, out int selectedIndex) &&
                selectedIndex >= 1 &&
                selectedIndex <= books.Docs.Count
            )
            {
                var chosenBook = books.Docs[selectedIndex - 1];

                PrintBookDetails(chosenBook);

                var shouldBeAdded = prompter.Ask(
                    """
                    Would you like to add this book to your library?
                    [Y]es
                    [N]o
                    
                    """
                );

                if (
                    shouldBeAdded.ToUpperInvariant() == "N" ||
                    string.IsNullOrEmpty(shouldBeAdded)
                )
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
                    "Your rating (0-5, or skip): ",
                    isOptional: true,
                    input => input is >= 0 and <= 5
                );

                var bookToBeAdded = new Book(
                    chosenBook.Title,
                    string.Join(", ", chosenBook.AuthorName) ?? "Unknown",
                    chosenBook.FirstPublishYear,
                    totalPages,
                    (ReadingStatus)readingStatus,
                    rating
                );

                await storage.Save(bookToBeAdded);

                Console.WriteLine("\nBook added successfully!");
            }
            else
            {
                Console.WriteLine("Invalid input. Please try again.");
            }

            consoleHelper.PressToContinue();
        }
    }

    private void PrintBookDetails(ExternalBook doc)
    {
        Console.WriteLine("\n========== BOOK DETAILS ==========");

        consoleHelper.PrintField(
            "Title",
            doc.Title ?? "Unknown Title"
        );

        var authors = doc.AuthorName is { Count: > 0 }
            ? string.Join(", ", doc.AuthorName)
            : "Unknown Author";
        consoleHelper.PrintField(
            "Author(s)",
            authors
        );

        if (doc.FirstPublishYear is not null)
        {
            consoleHelper.PrintField(
                "First published",
                doc.FirstPublishYear
            );
        }

        if (doc.Language is { Count: > 0 })
        {
            consoleHelper.PrintField(
                "Languages",
                string.Join(", ", doc.Language.Take(5))
            );
        }

        Console.WriteLine("==================================\n");
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

        var uriBuilder = new UriBuilder(_baseUrl)
        {
            Query = queryParams.ToString()
        };

        using var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
        request.Headers.UserAgent.ParseAdd("ReadingTracker/1.0 (144056146+TockePie@users.noreply.github.com)");

        using var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<OpenLibrarySearchResponse>();
    }

    private void DisplayBooks(IReadOnlyList<ExternalBook> books)
    {
        foreach (var (index, book) in books.Index())
        {
            var subtitle = book.Subtitle is not null ? $": {book.Subtitle}" : string.Empty;
            var authors = book.AuthorName is not null ? string.Join(", ", book.AuthorName) : "Unknown";

            Console.WriteLine(
                $"""
                    {index + 1}.  {book.Title}{subtitle}
                        Author: {string.Join(", ", authors)}
                        First published: {book.FirstPublishYear}
                        Open library ID: {book.CoverId}

                    """
            );
        }
    }
}
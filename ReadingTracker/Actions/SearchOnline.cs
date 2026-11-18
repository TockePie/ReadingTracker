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

        var books = await GetBooksAsync(userPrompt);

        for (int i = 0; i < 10; i++)
        {
            var book = books.Docs[i];
            var subtitle = book.Subtitle is not null ? $": {book.Subtitle}" : string.Empty;

            Console.WriteLine(
                $"""
                {i + 1}.  {book.Title}{subtitle}
                    Author: {string.Join(", ", book.AuthorName)}
                    First published: {book.FirstPublishYear}
                    Open library ID: {book.CoverId}

                """
            );
        }

        var selectedBook = prompter.AskInt(
            "Select a book: ",
            isOptional: true
        );
    }

    private async Task<OpenLibrarySearchResponse> GetBooksAsync(string query)
    {
        var queryParams = HttpUtility.ParseQueryString(string.Empty);
        queryParams["q"] = query;

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
}
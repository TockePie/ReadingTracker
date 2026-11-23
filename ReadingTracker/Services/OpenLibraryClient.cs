using System.Net.Http.Json;
using System.Web;
using ReadingTracker.Models;

namespace ReadingTracker.Services;

public class OpenLibraryClient(
    HttpClient httpClient,
    string baseUrl = "https://openlibrary.org/search.json"
) : IExternalLibrary
{
    public async Task<OpenLibrarySearchResponse> GetBooksAsync(
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
}
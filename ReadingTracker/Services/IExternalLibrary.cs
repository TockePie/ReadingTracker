using ReadingTracker.Models;

namespace ReadingTracker.Services;

public interface IExternalLibrary
{
    Task<OpenLibrarySearchResponse> GetBooksAsync(
        string query,
        int page = 1,
        int limit = 10
    );
}

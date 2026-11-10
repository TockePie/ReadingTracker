using ReadingTracker.Models;

namespace ReadingTracker.ConsoleHelper;

public interface IConsoleHelper
{
    void PrintField(string label, object? value);
    bool TryPromptBookDetails(IReadOnlyList<Book> books, out Book? book);
}

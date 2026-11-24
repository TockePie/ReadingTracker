namespace ReadingTracker.Prompter;

public interface IPrompter
{
    string Ask(string prompt);
    string Ask(string prompt, string defaultValue);

    int AskInt(
        string prompt,
        Func<int, bool>? validator = null,
        string? errorMessage = null
    );

    int? AskOptionalInt(
        string prompt,
        Func<int, bool>? validator = null,
        string? errorMessage = null
    );

    int? AskOptionalInt(
        string prompt,
        int? defaultValue,
        Func<int, bool>? validator = null,
        string? errorMessage = null
    );
}

namespace ReadingTracker.Prompter;

public interface IPrompter
{
    string Ask(string message);
    int AskInt(string message);
    int AskInt(string message, string errorMessage);
    int? AskInt(string message, bool isOptional);
    int? AskInt(string message, bool isOptional, string errorMessage);
    int AskInt(string message, Func<int, bool> condition);
    int AskInt(string message, Func<int, bool> condition, string errorMessage);
    int? AskInt(string message, bool isOptional, Func<int, bool> condition);
    int? AskInt(string message, bool isOptional, Func<int, bool> condition, string errorMessage);
}

namespace ReadingTracker.Prompter;

public class Prompter : IPrompter
{

    public string Ask(string message)
    {
        Console.Write(message);
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    public int AskInt(string message)
    {
        return GetInt(message, isOptional: false, condition: null, errorMessage: null)!.Value;
    }

    public int AskInt(string message, string errorMessage)
    {
        return GetInt(message, isOptional: false, condition: null, errorMessage)!.Value;
    }

    public int? AskInt(string message, bool isOptional)
    {
        return GetInt(message, isOptional, condition: null, errorMessage: null);
    }

    public int? AskInt(string message, bool isOptional, string errorMessage)
    {
        return GetInt(message, isOptional, condition: null, errorMessage);
    }

    public int AskInt(string message, Func<int, bool> condition)
    {
        return GetInt(message, isOptional: false, condition, errorMessage: null)!.Value;
    }

    public int AskInt(string message, Func<int, bool> condition, string errorMessage)
    {
        return GetInt(message, isOptional: false, condition, errorMessage)!.Value;
    }

    public int? AskInt(string message, bool isOptional, Func<int, bool> condition)
    {
        return GetInt(message, isOptional, condition, errorMessage: null);
    }

    public int? AskInt(string message, bool isOptional, Func<int, bool> condition, string errorMessage)
    {
        return GetInt(message, isOptional, condition, errorMessage);
    }

    private int? GetInt(
        string message,
        bool isOptional,
        Func<int, bool>? condition,
        string? errorMessage
    )
    {
        while (true)
        {
            var basePrompt = Ask(message);

            if (isOptional && string.IsNullOrWhiteSpace(basePrompt))
            {
                return null;
            }

            var isValid = int.TryParse(basePrompt, out int validated);
            if (isValid && (condition is null || condition(validated)))
            {
                return validated;
            }

            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                Console.WriteLine(errorMessage);
            }
            else
            {
                Console.WriteLine("You entered invalid number. Try again");
            }
        }
    }
}

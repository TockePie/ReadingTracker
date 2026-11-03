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
        return GetInt(message, isOptional: false, condition: null)!.Value;
    }

    public int? AskInt(string message, bool isOptional)
    {
        return GetInt(message, isOptional, condition: null);
    }

    public int AskInt(string message, Func<int, bool> condition)
    {
        return GetInt(message, isOptional: false, condition)!.Value;
    }

    public int? AskInt(string message, bool isOptional, Func<int, bool> condition)
    {
        return GetInt(message, isOptional, condition);
    }

    private int? GetInt(string message, bool isOptional, Func<int, bool>? condition)
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

            Console.WriteLine("You entered invalid number. Try again");
        }
    }
}

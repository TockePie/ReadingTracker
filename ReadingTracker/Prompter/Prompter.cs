namespace ReadingTracker.Prompter;

public class Prompter : IPrompter
{
    public string Ask(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    public string Ask(string prompt, string defaultValue)
    {
        var input = Ask($"{prompt} [{defaultValue}]: ");
        return input == string.Empty ? defaultValue : input;
    }

    public int AskInt(
        string prompt,
        Func<int, bool>? validator = null,
        string? errorMessage = null
    )
    {
        return GetInt(prompt, isOptional: false, validator, errorMessage)!.Value;
    }

    public int? AskOptionalInt(
        string prompt,
        Func<int, bool>? validator = null,
        string? errorMessage = null
    )
    {
        return GetInt(prompt, isOptional: true, validator, errorMessage);
    }

    public int? AskOptionalInt(
        string prompt,
        int? defaultValue,
        Func<int, bool>? validator = null,
        string? errorMessage = null
    )
    {
        var display = defaultValue?.ToString() ?? "No info";
        var input = GetInt($"{prompt} [{display}]: ", isOptional: true, validator, errorMessage);
        return input ?? defaultValue;
    }

    private int? GetInt(
        string prompt,
        bool isOptional,
        Func<int, bool>? validator,
        string? errorMessage
    )
    {
        while (true)
        {
            var input = Ask(prompt);

            if (isOptional && string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            var isNumber = int.TryParse(input, out int validated);
            if (isNumber && (validator is null || validator(validated)))
            {
                return validated;
            }

            if (!string.IsNullOrWhiteSpace(errorMessage))
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

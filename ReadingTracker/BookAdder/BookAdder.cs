namespace ReadingTracker.BookAdder;

public class BookAdder
{
    public void Add()
    {
        Console.WriteLine("========== ADD BOOK MANUALLY ==========");

        var title = PromptString("Title:");
        var author = PromptString("Author:");
        var publicationYear = PromptOptionalInt("Publication year:", input => input is > 0 and <= 2026);
        var totalPages = PromptOptionalInt("Total pages:");

        var selectedReadingStatus = PromptInt(@"
Reading status:
1. Want to Read
2. Reading
3. Completed
Select status:", input => input is >= 1 and < 4);

        var rating = PromptOptionalInt("\nYour rating (0-5, or skip): ");

        Console.WriteLine("\nBook added successfully!");
    }

    //TODO: Refactor later to use method chaining like this:
    // Prompt("message").ToInt().WithCondition(...).Optional();
    private string PromptString(string message)
    {
        Console.Write(message);
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    private int PromptInt(string message)
    {
        return PromptInt(message, condition: null);
    }

    private int PromptInt(string message, Func<int, bool>? condition)
    {
        int validated;

        while (true)
        {
            var basePrompt = PromptString(message);
            var isValid = int.TryParse(basePrompt, out validated);

            if (isValid && (condition is null || condition(validated)))
            {
                break;
            }

            Console.WriteLine("You entered invalid number. Try again");
        }

        return validated;
    }

    private int? PromptOptionalInt(string message)
    {
        return PromptOptionalInt(message, condition: null);
    }

    private int? PromptOptionalInt(string message, Func<int, bool>? condition)
    {
        int validated;

        while (true)
        {
            var basePrompt = PromptString(message);
            var isValid = int.TryParse(basePrompt, out validated);

            if (basePrompt == string.Empty ||
                isValid && (condition is null ||
                condition(validated))
            )
            {
                break;
            }

            Console.WriteLine("You entered invalid number. Try again");
        }

        return validated;
    }
}
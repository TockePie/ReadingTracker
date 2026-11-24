using ReadingTracker.Actions;
using ReadingTracker.ConsoleHelper;
using ReadingTracker.Exporters;
using ReadingTracker.Prompter;
using ReadingTracker.Services;
using ReadingTracker.Storage;

var prompter = new Prompter();
var jsonStorage = new JsonStorage("books.json");
var consoleHelper = new ConsoleHelper(prompter);
var externalLibrary = new OpenLibraryClient(new HttpClient());
var exporters = new List<IExporter>()
{
    new CsvExporter(),
    new JsonExporter()
};

var menuActions = new Dictionary<int, IMenuAction>
{
    { 1, new MyLibrary(prompter, jsonStorage, consoleHelper) },
    { 2, new SearchOnline(externalLibrary, prompter, jsonStorage, consoleHelper) },
    { 3, new BookAdder(prompter, jsonStorage, consoleHelper) },
    { 4, new UpdateReadingProgress(prompter, jsonStorage, consoleHelper) },
    { 5, new ChangeReadingStatus(prompter, jsonStorage, consoleHelper) },
    { 6, new SearchAndFilterBooks(prompter, jsonStorage, consoleHelper)},
    { 7, new ReadingStatistics(jsonStorage, consoleHelper) },
    { 8, new ExportLibrary(prompter, jsonStorage, consoleHelper, exporters) },
    { 0, new ExitAction() }
};

var runner = new Runner(menuActions, prompter);
await runner.Run();

class Runner(IReadOnlyDictionary<int, IMenuAction> menuActions, IPrompter prompter)
{
    public async Task Run()
    {
        while (true)
        {
            DisplayEntryMessage();

            var selectedOption = prompter.AskInt(
                "Select an option: ",
                menuActions.ContainsKey,
                errorMessage: "Select an option between 0 and 8"
            );

            var shouldContinue = await ExecuteSafelyAsync(
                () => menuActions[selectedOption].ExecuteAsync()
            );

            if (!shouldContinue)
            {
                break;
            }
        }
    }

    private void DisplayEntryMessage()
    {
        Console.WriteLine(
            """
            ================================
                PERSONAL READING TRACKER
            ================================
            
            """
        );

        var sortedActions = menuActions
            .OrderBy(action => action.Key == 0 ? 1 : 0)
            .ThenBy(action => action.Key);

        foreach (var action in sortedActions)
        {
            Console.WriteLine($"{action.Key}. {action.Value.Description}");
        }

        Console.WriteLine();
    }

    private async Task<bool> ExecuteSafelyAsync(Func<Task<bool>> action)
    {
        try
        {
            return await action();
        }
        catch (StorageException ex)
        {
            Console.WriteLine($"\nStorage Error: {ex.Message}\n");
            return true;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"\nError: {ex.Message}\n");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nUnexpected Error: {ex.Message}\n");
            return true;
        }
    }
}

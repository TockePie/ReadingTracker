using ReadingTracker.Actions;
using ReadingTracker.ConsoleHelper;
using ReadingTracker.Prompter;
using ReadingTracker.Storage;

var prompter = new Prompter();
var jsonStorage = new JsonStorage("books.json");
var consoleHelper = new ConsoleHelper(prompter);

var menuActions = new Dictionary<int, IMenuAction>
{
    { 1, new MyLibrary(prompter, jsonStorage, consoleHelper) },
    { 2, new NotImplementedMenuAction("Search books online") },
    { 3, new BookAdder(prompter, jsonStorage) },
    { 4, new UpdateReadingProgress(prompter, jsonStorage, consoleHelper) },
    { 5, new NotImplementedMenuAction("Change reading status") },
    { 6, new NotImplementedMenuAction("Search and filter my books") },
    { 7, new NotImplementedMenuAction("Reading statistics") },
    { 8, new NotImplementedMenuAction("Export library") },
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

            await menuActions[selectedOption].ExecuteAsync();
        }
    }

    private void DisplayEntryMessage()
    {
        Console.WriteLine(@"
================================
     PERSONAL READING TRACKER
================================
");

        var sortedActions = menuActions
            .OrderBy(action => action.Key == 0 ? 1 : 0)
            .ThenBy(action => action.Key);

        foreach (var action in sortedActions)
        {
            Console.WriteLine($"{action.Key}. {action.Value.Description}");
        }

        Console.WriteLine();
    }
}

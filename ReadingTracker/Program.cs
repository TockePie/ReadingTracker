using ReadingTracker;
using ReadingTracker.BookAdderAction;
using ReadingTracker.MyLibraryAction;
using ReadingTracker.Prompter;
using ReadingTracker.Storage;
using ReadingTracker.UpdateReadingProgressAction;

var prompter = new Prompter();
var jsonStorage = new JsonStorage("books.json");

var menuActions = new Dictionary<int, IMenuAction>
{
    { 1, new MyLibrary(prompter, jsonStorage) },
    { 2, new NotImplementedMenuAction("Search books online") },
    { 3, new BookAdder(prompter, jsonStorage) },
    { 4, new UpdateReadingProgress(prompter, jsonStorage) },
    { 5, new NotImplementedMenuAction("Change reading status") },
    { 6, new NotImplementedMenuAction("Search and filter my books") },
    { 7, new NotImplementedMenuAction("Reading statistics") },
    { 8, new NotImplementedMenuAction("Export library") },
    { 0, new ExitAction() }
};

var runner = new Runner(menuActions);
runner.Run();

class Runner(Dictionary<int, IMenuAction> menuActions)
{
    public void Run()
    {
        while (true)
        {
            DisplayEntryMessage();

            int selectedOption;

            Console.WriteLine("Select an option:");

            while (true)
            {
                var userInput = Console.ReadLine();
                var isValid = int.TryParse(userInput, out selectedOption);

                if (isValid && menuActions.ContainsKey(selectedOption))
                {
                    break;
                }

                Console.WriteLine("Select an option between 0 and 8:");
            }

            menuActions[selectedOption].Execute();
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

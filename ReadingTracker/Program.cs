using ReadingTracker;
using ReadingTracker.BookAdderAction;
using ReadingTracker.MyLibraryAction;
using ReadingTracker.Prompter;
using ReadingTracker.Storage;
using ReadingTracker.UpdateReadingProgressAction;

var notImplemented = new NotImplementedMenuAction();
var prompter = new Prompter();
var jsonStorage = new JsonStorage("books.json");

var menuActions = new Dictionary<int, IMenuAction>
{
    { 0, notImplemented },
    { 1, new MyLibrary(prompter, jsonStorage) },
    { 2, notImplemented },
    { 3, new BookAdder(prompter, jsonStorage) },
    { 4, new UpdateReadingProgress(prompter, jsonStorage) },
    { 5, notImplemented },
    { 6, notImplemented },
    { 7, notImplemented },
    { 8, notImplemented }
};

DisplayEntryMessage();

//TODO: Move selection to a method
int selectedOption;

Console.WriteLine("Select an option:");

while (true)
{
    var userInput = Console.ReadLine();
    var isValid = int.TryParse(userInput, out selectedOption);

    if (isValid && selectedOption is >= 0 and <= 8)
    {
        break;
    }

    Console.WriteLine("Select an option between 0 and 8:");
}

menuActions[selectedOption].Execute();

//XXX: Methods. Should be splitted into their own classes later.

void DisplayEntryMessage()
{
    Console.WriteLine(@"
================================
     PERSONAL READING TRACKER
================================

1. My library
2. Search books online
3. Add a book manually
4. Update reading progress
5. Change reading status
6. Search and filter my books
7. Reading statistics
8. Export library
0. Exit
");
}

Console.ReadKey();

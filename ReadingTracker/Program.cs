using ReadingTracker.BookAdder;

var bookAdder = new BookAdder();

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

switch (selectedOption)
{
    case 1:
    case 2:
    case 4:
    case 5:
    case 6:
    case 7:
    case 8:
    case 0:
        {
            throw new NotImplementedException();
        }

    case 3:
        {
            bookAdder.Add();
            break;
        }
}

//XXX: Methods. Should be splitted into their own classes later.
{
    Console.WriteLine("========== ADD BOOK MANUALLY ==========");
    Console.WriteLine("Title:");
    var title = Console.ReadLine();

    Console.WriteLine("Author:");
    var author = Console.ReadLine();

    Console.WriteLine("Publication year:");
    var publicatioYear = Console.ReadLine();

    Console.WriteLine("Total pages:");
    var totalPages = Console.ReadLine();

    Console.WriteLine(@"
Reading status:
1. Want to Read
2. Reading
3. Completed
Select status:");
    var selectedReadingStatus = Console.ReadLine();

    Console.WriteLine("\nYour rating (0-5, or skip): ");
    var rating = Console.ReadLine();

    Console.WriteLine("\nBook added successfully!");
}

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

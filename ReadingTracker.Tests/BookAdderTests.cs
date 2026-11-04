using Moq;
using ReadingTracker.BookAdderAction;
using ReadingTracker.Prompter;
using ReadingTracker.Storage;

namespace ReadingTracker.Tests;

[TestFixture]
public class BookAdderTests
{
    private BookAdder _bookAdder;
    private Mock<IPrompter> _mockPrompter = new();
    private Mock<IStorage> _mockStorage = new();

    [SetUp]
    public void SetUp()
    {
        _mockPrompter = new Mock<IPrompter>();
        _mockStorage = new Mock<IStorage>();

        _bookAdder = new BookAdder(_mockPrompter.Object, _mockStorage.Object);
    }

    private void SetupAskIntToAlwaysReturn(
        int? year,
        int? totalPages,
        int readingStatus,
        int? rating
    )
    {
        _mockPrompter.Setup(fn => fn.AskInt(
            "Publication year:",
            It.IsAny<bool>(),
            It.IsAny<Func<int, bool>>()
        )).Returns(year);

        _mockPrompter.Setup(fn => fn.AskInt(
            "Total pages:",
            It.IsAny<bool>()
        )).Returns(totalPages);

        _mockPrompter.Setup(fn => fn.AskInt(
            It.Is<string>(s => s.Contains("Reading status:")),
            It.IsAny<Func<int, bool>>()
        )).Returns(readingStatus);

        _mockPrompter.Setup(fn => fn.AskInt(
            "\nYour rating (0-5, or skip):",
            It.IsAny<bool>(),
            It.IsAny<Func<int, bool>>()
        )).Returns(rating);
    }

    [TestCase("The Hobbit", "J.R.R. Tolkien", 1920, 123, 1, null)]
    public void Execute_ShouldSaveBookToStorage(
        string title,
        string author,
        int? year,
        int? totalPages,
        int readingStatus,
        int? rating
    )
    {
        _mockPrompter.Setup(fn => fn.Ask("Title:")).Returns(title);
        _mockPrompter.Setup(fn => fn.Ask("Author:")).Returns(author);

        SetupAskIntToAlwaysReturn(
            year,
            totalPages,
            readingStatus,
            rating
        );

        _mockStorage
            .Setup(fn => fn.Save(It.IsAny<string>(), It.IsAny<Book>()))
            .Returns(Task.CompletedTask);

        _bookAdder.Execute();

        _mockStorage.Verify(
            fn => fn.Save("books.json", It.Is<Book>(b =>
                b.Title == title &&
                b.Author == author &&
                b.PublicationYear == year &&
                b.TotalPages == totalPages &&
                b.Status == (ReadingStatus)(readingStatus - 1) &&
                b.Rating == rating
            )),
            Times.Once
        );
    }
}
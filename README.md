# 📚 Personal Reading Tracker

A console-based application built with **C# and .NET** for managing a personal book library, tracking reading progress, exploring books through the Open Library API, and analyzing reading habits.

This project was developed as a personal learning project to practice and apply core C# and software development concepts in a practical application.

```text
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

Select an option:
```

## Features

- Personal Library Management - Add books to your personal library and keep track of your collection.

- Reading Status Tracking - Organize books by reading status: `Want To Read`, `Reading`, and `Completed`.

- Reading Progress - Track the current page and calculate reading progress.

- Book Ratings - Rate books and use ratings in your reading statistics.

- Online Book Search - Search for books using the Open Library API.

- Search & Filtering - Search your library, filter books by reading status, and sort your collection.

- Reading Statistics - View statistics about your library and reading activity.

- Data Persistence - Store your library locally using JSON.

- CSV Export - Export your book collection to CSV.

- Input Validation - Handle invalid input and provide a more user-friendly console experience.

- _Testing files will be added very soon._

## Example Usage

### Library View

```text
========== MY LIBRARY ==========

ID: 1 | Project Hail Mary - Andy Weir
        Status: Completed | Progress: 496/496 pages (100%)

ID: 2 | The Pragmatic Programmer - Andrew Hunt & David Thomas
        Status: Reading | Progress: 140/352 pages (39.8%)

ID: 3 | The Three-Body Problem - Cixin Liu
        Status: Want to read | Progress: 0/400 pages (0%)

ID: 4 | The Winds of Winter - George R.R. Martin
        Status: Want to read | Progress: 0 pages

ID: 5 | Infinite Jest - David Foster Wallace
        Status: Reading | Progress: 215/1079 pages (19.9%)

ID: 6 | Clean Code - Robert C. Martin
        Status: Reading | Progress: 85 pages

--------------------------------

1. View book details
2. Edit book information
3. Remove a book
4. Back to main menu

Select an option:
```

### Searching Online

```text
========== SEARCH BOOKS ONLINE ==========

Enter book title or author: Dune
Looking for books (Page: 1)...

1.  Dune
    Author: Frank Herbert
    First published: 1965
    Open library ID: /works/OL893414W

2.  Dune Messiah
    Author: Frank Herbert
    First published: 1969
    Open library ID: /works/OL893461W

3.  Children of Dune
    Author: Frank Herbert
    First published: 1976
    Open library ID: /works/OL893516W

...

1-10: Select book
[N]ext page
[C]ancel

Select an option: 1

========== BOOK DETAILS ==========
Title:               Dune
Author(s):           Frank Herbert
First published:     1965
Languages:           ukr, rus, fre, pol, por
==================================

Would you like to add this book to your library?
[Y]es | [N]o
y
Total pages:
Reading status:
1. Want to Read
2. Reading
3. Completed
Select status: 1
Your rating (1-5, or skip): 4

Book added successfully!
Press any key to continue...
```

### Reading Statistics

```text
========== READING STATISTICS ==========

Total books:         7
Want to Read:        3
Currently reading:   3
Completed:           1

Total pages read:    936
Pages remaining in library: 1476
Average rating:      3.8

Author with most completed books:
Andy Weir - 1 books completed

Press any key to continue...
```

## Tech Stack

- Language: C#
- Platform: .NET
- Data Storage: JSON
- External API: Open Library API
- Libraries: `System.Text.Json`, `HttpClient`
- Testing: NUnit & Moq _(when the test project is added)_

## Project Structure

```text
ReadingTracker/
├── Actions/          # Application actions and features
├── ConsoleHelper/    # Console UI helpers
├── Extensions/       # Extension methods
├── Models/           # Domain models and enums
├── Prompter/         # User input and validation
├── Storage/          # Data persistence and export
├── Program.cs        # Application entry point
└── ...
```

## Getting Started

### Prerequisites

- [.NET SDK version 10](https://dotnet.microsoft.com/download)
- Git

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/TockePie/ReadingTracker.git
   ```

2. Navigate to the project directory:

   ```bash
   cd ReadingTracker
   ```

3. Run the application:

   ```bash
   dotnet run --project ReadingTracker
   ```

## Data Storage

The application stores the personal book library locally in JSON format.

This allows the collection to persist between application runs without requiring a database server or external infrastructure.

CSV export is available for users who want to save their collection in a format that can be opened in spreadsheet applications.

## External API

The application uses the **[Open Library API](https://openlibrary.org/developers/api)** to search for books online.

The API is used to retrieve book information and demonstrate integration with an external service using `HttpClient` and asynchronous operations.

## Concepts Practiced

This project was built to reinforce practical C# and software development fundamentals, including:

- Object-Oriented Programming (OOP)
- Classes, records, interfaces, and enums
- Encapsulation and separation of responsibilities
- Generics
- LINQ
- Extension methods
- Asynchronous programming (`async` / `await`)
- HTTP requests and API integration
- JSON serialization and deserialization
- File I/O
- Exception handling
- Dependency injection and dependency inversion principles
- Unit testing _(in progress)_

## Project Goals

The main goals of this project were to:

- Consolidate knowledge of C# fundamentals through hands-on development.
- Practice designing a small application with separated responsibilities.
- Work with local data persistence.
- Integrate an external REST API.
- Improve problem-solving and code organization.
- Learn to write and maintain automated tests.

## License

This project is intended for educational and portfolio purposes.

using LibraryData;

class Program
{
    static BookRepository bookRepo = new BookRepository();
    static MemberRepository memberRepo = new MemberRepository();
    static LibraryService service = new LibraryService();
    static BackupHelper backup = new BackupHelper();

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\n1. Add Book");
            Console.WriteLine("2. View All Books");
            Console.WriteLine("3. Add Member");
            Console.WriteLine("4. View All Members");
            Console.WriteLine("5. Issue Book to Member (Transaction)");
            Console.WriteLine("6. Return Book (Transaction)");
            Console.WriteLine("7. Export Books to JSON");
            Console.WriteLine("8. Import Books from JSON");
            Console.WriteLine("9. Exit");
            Console.Write("Choose: ");

            try
            {
                switch (Console.ReadLine())
                {
                    case "1": AddBook(); break;
                    case "2": ViewBooks(); break;
                    case "3": AddMember(); break;
                    case "4": ViewMembers(); break;
                    case "5": Issue(); break;
                    case "6": Return(); break;
                    case "7": Export(); break;
                    case "8": Import(); break;
                    case "9": return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }

    static int ReadInt(string prompt)
    {
        int value;
        Console.Write(prompt);
        while (!int.TryParse(Console.ReadLine(), out value))
            Console.Write("Enter a valid number: ");
        return value;
    }

    static string ReadText(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    static void AddBook()
    {
        Book b = new Book
        {
            Title = ReadText("Title: "),
            Author = ReadText("Author: "),
            Stock = ReadInt("Stock: ")
        };
        bookRepo.AddBook(b);
        Console.WriteLine("Book added.");
    }

    static void ViewBooks()
    {
        foreach (Book b in bookRepo.GetAllBooks())
            Console.WriteLine($"{b.BookId} | {b.Title} | {b.Author} | Stock: {b.Stock}");
    }

    static void AddMember()
    {
        memberRepo.AddMember(new Member { Name = ReadText("Name: ") });
        Console.WriteLine("Member added.");
    }

    static void ViewMembers()
    {
        foreach (Member m in memberRepo.GetAllMembers())
            Console.WriteLine($"{m.MemberId} | {m.Name}");
    }

    static void Issue()
    {
        int bookId = ReadInt("Book Id: ");
        int memberId = ReadInt("Member Id: ");
        Console.WriteLine(service.IssueBook(bookId, memberId)
            ? "Book issued successfully."
            : "Could not issue book (no copies, or invalid book/member). Nothing was saved.");
    }

    static void Return()
    {
        int issueId = ReadInt("Issue Id: ");
        Console.WriteLine(service.ReturnBook(issueId)
            ? "Book returned successfully."
            : "Could not return book (invalid or already returned). Nothing was saved.");
    }

    static void Export()
    {
        string path = ReadText("File path (e.g. books.json): ");
        backup.SaveBooks(bookRepo.GetAllBooks(), path);
        Console.WriteLine("Exported.");
    }

    static void Import()
    {
        string path = ReadText("File path: ");
        List<Book> books = backup.LoadBooks(path);
        foreach (Book b in books)
            Console.WriteLine($"{b.Title} - Stock: {b.Stock}");
    }
}

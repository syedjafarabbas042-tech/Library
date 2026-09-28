namespace LibraryData
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Stock { get; set; }
    }

    public class Member
    {
        public int MemberId { get; set; }
        public string Name { get; set; }
    }
}

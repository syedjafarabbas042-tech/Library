using Microsoft.Data.SqlClient;

namespace LibraryData
{
    public class BookRepository
    {
        public void AddBook(Book book)
        {
            using SqlConnection con = new SqlConnection(DbConfig.ConnectionString);
            using SqlCommand cmd = new SqlCommand(
                "INSERT INTO Books (Title, Author, Stock) VALUES (@title, @author, @stock)", con);
            cmd.Parameters.AddWithValue("@title", book.Title);
            cmd.Parameters.AddWithValue("@author", book.Author);
            cmd.Parameters.AddWithValue("@stock", book.Stock);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        public List<Book> GetAllBooks()
        {
            List<Book> books = new List<Book>();
            using SqlConnection con = new SqlConnection(DbConfig.ConnectionString);
            using SqlCommand cmd = new SqlCommand("SELECT BookId, Title, Author, Stock FROM Books", con);
            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                books.Add(new Book
                {
                    BookId = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Author = reader.GetString(2),
                    Stock = reader.GetInt32(3)
                });
            }
            return books;
        }

        public bool UpdateStock(int bookId, int newStock)
        {
            using SqlConnection con = new SqlConnection(DbConfig.ConnectionString);
            using SqlCommand cmd = new SqlCommand(
                "UPDATE Books SET Stock = @stock WHERE BookId = @id", con);
            cmd.Parameters.AddWithValue("@stock", newStock);
            cmd.Parameters.AddWithValue("@id", bookId);
            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DeleteBook(int bookId)
        {
            try
            {
                using SqlConnection con = new SqlConnection(DbConfig.ConnectionString);
                using SqlCommand cmd = new SqlCommand("DELETE FROM Books WHERE BookId = @id", con);
                cmd.Parameters.AddWithValue("@id", bookId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (SqlException)
            {
                // e.g. FK violation: book has issue records
                return false;
            }
        }
    }
}

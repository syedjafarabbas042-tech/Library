using Microsoft.Data.SqlClient;

namespace LibraryData
{
    public class LibraryService
    {
        // Part C
        public bool IssueBook(int bookId, int memberId)
        {
            using SqlConnection con = new SqlConnection(DbConfig.ConnectionString);
            con.Open();
            using SqlTransaction tx = con.BeginTransaction();
            try
            {
                // 1. Check a copy is available (UPDLOCK stops others changing it mid-transaction)
                int stock;
                using (SqlCommand check = new SqlCommand(
                    "SELECT Stock FROM Books WITH (UPDLOCK) WHERE BookId = @bookId", con, tx))
                {
                    check.Parameters.AddWithValue("@bookId", bookId);
                    object result = check.ExecuteScalar();
                    if (result == null) { tx.Rollback(); return false; }   // book doesn't exist
                    stock = (int)result;
                }
                if (stock <= 0) { tx.Rollback(); return false; }           // no copies left

                // 2. Record who borrowed what and when
                using (SqlCommand insert = new SqlCommand(
                    "INSERT INTO IssuedBooks (BookId, MemberId, IssueDate) VALUES (@bookId, @memberId, @date)", con, tx))
                {
                    insert.Parameters.AddWithValue("@bookId", bookId);
                    insert.Parameters.AddWithValue("@memberId", memberId);   // bad member -> FK error -> rollback
                    insert.Parameters.AddWithValue("@date", DateTime.Now);
                    insert.ExecuteNonQuery();
                }

                // 3. One fewer copy available
                using (SqlCommand update = new SqlCommand(
                    "UPDATE Books SET Stock = Stock - 1 WHERE BookId = @bookId", con, tx))
                {
                    update.Parameters.AddWithValue("@bookId", bookId);
                    update.ExecuteNonQuery();
                }

                tx.Commit();
                return true;
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                return false;
            }
        }

        // Part D
        public bool ReturnBook(int issueId)
        {
            using SqlConnection con = new SqlConnection(DbConfig.ConnectionString);
            con.Open();
            using SqlTransaction tx = con.BeginTransaction();
            try
            {
                // 1. Find the issue record and make sure it's still an open loan
                int bookId;
                using (SqlCommand find = new SqlCommand(
                    "SELECT BookId, ReturnDate FROM IssuedBooks WITH (UPDLOCK) WHERE IssueId = @issueId", con, tx))
                {
                    find.Parameters.AddWithValue("@issueId", issueId);
                    using SqlDataReader reader = find.ExecuteReader();
                    if (!reader.Read()) { reader.Close(); tx.Rollback(); return false; }  // doesn't exist
                    if (!reader.IsDBNull(1)) { reader.Close(); tx.Rollback(); return false; } // already returned
                    bookId = reader.GetInt32(0);
                }

                // 2. Record the return
                using (SqlCommand ret = new SqlCommand(
                    "UPDATE IssuedBooks SET ReturnDate = @date WHERE IssueId = @issueId", con, tx))
                {
                    ret.Parameters.AddWithValue("@date", DateTime.Now);
                    ret.Parameters.AddWithValue("@issueId", issueId);
                    ret.ExecuteNonQuery();
                }

                // 3. One more copy available
                using (SqlCommand update = new SqlCommand(
                    "UPDATE Books SET Stock = Stock + 1 WHERE BookId = @bookId", con, tx))
                {
                    update.Parameters.AddWithValue("@bookId", bookId);
                    update.ExecuteNonQuery();
                }

                tx.Commit();
                return true;
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                return false;
            }
        }
    }
}

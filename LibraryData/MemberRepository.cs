using Microsoft.Data.SqlClient;

namespace LibraryData
{
    public class MemberRepository
    {
        public void AddMember(Member member)
        {
            using SqlConnection con = new SqlConnection(DbConfig.ConnectionString);
            using SqlCommand cmd = new SqlCommand("INSERT INTO Members (Name) VALUES (@name)", con);
            cmd.Parameters.AddWithValue("@name", member.Name);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        public List<Member> GetAllMembers()
        {
            List<Member> members = new List<Member>();
            using SqlConnection con = new SqlConnection(DbConfig.ConnectionString);
            using SqlCommand cmd = new SqlCommand("SELECT MemberId, Name FROM Members", con);
            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                members.Add(new Member { MemberId = reader.GetInt32(0), Name = reader.GetString(1) });
            }
            return members;
        }
    }
}

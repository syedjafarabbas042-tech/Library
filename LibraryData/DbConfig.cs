namespace LibraryData
{
    public static class DbConfig
    {
        // Change this to match your SQL Server / LocalDB instance
        public const string ConnectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=LibraryDb;Trusted_Connection=True;Encrypt=False;";
    }
}

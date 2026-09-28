using System.Text.Json;

namespace LibraryData
{
    public class BackupHelper
    {
        public void SaveBooks(List<Book> books, string filePath)
        {
            string json = JsonSerializer.Serialize(books, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public List<Book> LoadBooks(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
        }
    }
}

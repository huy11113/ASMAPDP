using ASMAPDP.Models;

namespace ASMAPDP.Common
{
    public class CSVService : DatabaseService, IReadWriteUser
    {
        public List<User> ReadUser()
        {
            return ReadData().Where(row => row[0] == "User")
                .Select(row => new User
                {
                    Id = int.Parse(row[1]),
                    FullName = row[2],
                    Address = row[3],
                    username = row[4],
                    password = row[5],
                    role = int.Parse(row[6])
                })
                .ToList();
        }
        public void WriteUser(List<User> users)
        {
            var allData = ReadData().Where(row => row[0] != "User").ToList();
            allData.AddRange(users.Select(users => new[]
            {
                "User",
            u.Id.ToString(),
            u.FullName,
            u.Address,
            u.username,
            u.password,
            u.role.ToString()
            }));
            WriteData(allData);
        }
        private List<string[]> ReadData()
        {
            var rows = new List<string[]>();
            if (!File.Exists(_filePath)) return rows;

            using (var reader = new StreamReader(_filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        rows.Add(line.Split(','));
                    }
                }
                reader.Close();
            }
            return rows;
        }
        private void WriteData()
        {
            using (var writer = new StreamWriter(_filePath))
            {
                foreach (var row in data)
                {
                    writer.WriteLine(string.Join(",", row));
                }
                writer.Close();
            }
        }
    }
}

using System.Text;

namespace DbAccessPoint.Database
{
    public static class DatabaseConfig
    {
        public static Dictionary<string, object> ReadConfig()
        {
            var connectionData = new StringBuilder(string.Empty);

            if (!File.Exists(".env"))
                throw new FileNotFoundException("There is no environment file in root to read database configuration.");

            using (var reader = new StreamReader(".env", Encoding.UTF8))
            {
                string? line = reader.ReadLine();

                while (line is not null)
                {
                    connectionData.Append($";{line.TrimEnd()}");
                }
            }

            return new Dictionary<string, object>
            {
                { "dbConnection", connectionData.ToString() },
                { "maxRetries", 3 }
            };
        }
    }
}

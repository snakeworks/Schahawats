using System.Data.SQLite;

namespace ChessUI
{
    public static class DatabaseHandler
    {
        private const string DATA_SOURCE = "GamesDatabase.db";
        private const string CONNECTION = $"Data Source={DATA_SOURCE};Version=3;";
        private const string TABLE_NAME = "Games";

        private static ChessGame CreateChessGame(SQLiteDataReader reader)
        {
            return new()
            {
                Id = reader.GetInt32(0),
                Date = GetDateFromString(reader.GetString(1)),
                WhiteName = reader.GetString(2),
                BlackName = reader.GetString(3),
                FullPgn = reader.GetString(4)
            };
        }

        public static IEnumerable<ChessGame> GetAllGames()
        {
            if (!System.IO.File.Exists(DATA_SOURCE) || !TableExists())
            {
                yield break;
            }

            string query = $"SELECT * FROM {TABLE_NAME}";

            using SQLiteConnection connection = new(CONNECTION);
            connection.Open();

            using (SQLiteCommand command = new(query, connection))
            {
                using SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        yield return CreateChessGame(reader);
                    }
                }
            }

            connection.Close();
        }
        private static DateOnly GetDateFromString(string dateString)
        {
            string[] split = dateString.Split('.');
            return new(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
        }
        private static bool TableExists()
        {
            using SQLiteConnection connection = new(CONNECTION);
            connection.Open();

            using SQLiteCommand command = new($"SELECT name FROM sqlite_master WHERE type='table' AND name='{TABLE_NAME}'", connection);
            
            object result = command.ExecuteScalar();
            return result != null;
        }
    }
}

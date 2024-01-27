using System.Data.SQLite;

namespace ChessUI
{
    public static class DatabaseHandler
    {
        private const string DATA_SOURCE = "GamesDatabase.db";
        private const string CONNECTION = $"Data Source={DATA_SOURCE};Version=3;";

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

        public static ChessGame GetGame(int index)
        {
            string query = $"SELECT * FROM Games WHERE Id = {index}";

            using SQLiteConnection connection = new(CONNECTION);
            connection.Open();

            using (SQLiteCommand command = new(query, connection))
            {
                using SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        var game = CreateChessGame(reader);
                        connection.Close();
                        return game;
                    }
                }
            }

            connection.Close();

            return null;
        }
        public static IEnumerable<ChessGame> GetAllGames()
        {
            string query = "SELECT * FROM Games";

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
    }
}

using System.Data.SQLite;
using System.Diagnostics;

namespace ChessUI
{
    public static class DatabaseHandler
    {
        private const string DATA_SOURCE = "GamesDatabase.db";
        private const string CONNECTION = $"Data Source={DATA_SOURCE};Version=3;";

        public static void GetAllGames()
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
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Debug.WriteLine(reader.GetName(i) + ": " + reader.GetValue(i));
                        }
                        Debug.WriteLine("");
                    }
                }
                else
                {
                    Debug.WriteLine("No rows found.");
                }
            }

            connection.Close();
        }
    }
}

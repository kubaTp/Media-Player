using System.IO;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Media_Player
{
    public class SqlCon
    {
        SQLiteConnection m_dbConnection;
        int id = 0;

        public void IfDatabaseExist(string baza)
        {
            if (File.Exists(baza))
            {
                ConnectToDatabase(baza);
            }
            else
            {
                SQLiteConnection.CreateFile(baza);
                ConnectToDatabase(baza);
                CreateTable();
            }
        }

        public void ConnectToDatabase(string baza)
        {
            m_dbConnection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", baza));
            m_dbConnection.Open();
        }
        public void CreateTable()
        {
            string sql = "create table filmy (Id int, Nazwa varchar(90))";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        public void CheckLast()
        {
            string selectMaxId = "Select Max(Id) From filmy";
            SQLiteCommand selectMaxCmd = new SQLiteCommand(selectMaxId, m_dbConnection);
            object val = selectMaxCmd.ExecuteScalar();
            int.TryParse(val.ToString(), out id);
        }
        public void AddToSql(string _nazwa)
        {
            CheckLast();
            id++;
            string sql = string.Format("INSERT INTO filmy(Id, Nazwa) VALUES('{0}','{1}')", id, _nazwa);
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }
        public void CloseConnection()
        {
            m_dbConnection.Close();
        }
        public List<DisplayMovies.Produkt> getFromTable(string baza)
        {
            List<DisplayMovies.Produkt> produkty = new List<DisplayMovies.Produkt>();
            ConnectToDatabase(baza);
            string sql = "SELECT * FROM filmy";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    DisplayMovies.Produkt element = new DisplayMovies.Produkt
                    {
                        Name = (string)reader["Nazwa"],
                    };
                    produkty.Add(element);
                }
            }
            return produkty;
        }
    }
}

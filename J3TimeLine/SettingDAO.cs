using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;

namespace J3TimeLine
{
    class SettingDAO
    {
        static string dbPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "setup", "j3timeline.db");
        static SQLiteConnection connection = new SQLiteConnection("datasource = " + dbPath);

        static public string getValueByKey(string key)
        {
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "select * from Setting where key = ?";
                var p = new SQLiteParameter();
                command.Parameters.Add(p);
                p.Value = key;
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return reader.GetString(1);
                }
                else return null;
            }
        }

        static public void updateValueByKey(string key, string newValue)
        {
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();
            using (var tr = connection.BeginTransaction())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE Setting SET value=120 WHERE key='DEFAULTX'";
                    //command.Parameters.Add("value", System.Data.DbType.AnsiString).Value = newValue;
                    //command.Parameters.Add("key", System.Data.DbType.AnsiString).Value = key;
                    Console.WriteLine(command.ExecuteNonQuery());
                }
                tr.Commit();
                tr.Dispose();
            }
        }
    }
}

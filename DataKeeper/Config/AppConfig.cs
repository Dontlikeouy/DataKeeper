using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DataKeeper.Config
{
    class AppConfig
    {
        static public string path = "database.db";
        static public string cs = $"Data Source={path}";
        static public SQLiteConnection con = new SQLiteConnection(cs);
        static public SQLiteCommand cmd = new SQLiteCommand(con);
        static public SQLiteDataAdapter adapter = new SQLiteDataAdapter(AppConfig.cmd);

        static public async Task<bool> executeRequest(string request,bool error=true,bool accept=true)
        {
            try
            {
                cmd.CommandText = request;
                await cmd.ExecuteNonQueryAsync();
                if (accept)
                {
                    MessageBox.Show($"Выполннено успешно");
                }
                return true;
            }
            catch (Exception ex)
            {
                if (error)
                {
                    MessageBox.Show($"Произошла ошибка:\n{ex.Message.ToString()}");
                }
                return false;
            }
        }

        static public bool executeRequestSync(string request, bool error = true, bool accept = true)
        {
            try
            {
                cmd.CommandText = request;
                cmd.ExecuteNonQuery();
                if (accept)
                {
                    MessageBox.Show($"Выполннено успешно");
                }
                return true;
            }
            catch (Exception ex)
            {
                if (error)
                {
                    MessageBox.Show($"Произошла ошибка:\n{ex.Message.ToString()}");
                }
                return false;
            }
        }
    }
}

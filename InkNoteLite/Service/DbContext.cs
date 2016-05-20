using InkNoteLite.Model;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkNoteLite.Service
{
    public class DbContext
    {
        private static readonly string DbFileName = "InkNoteLite.db";
        private static string DbFilePath;

        public static void Init()
        {
            DbFilePath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, DbFileName);
            using (var db = GetDbConnection())
            {
                db.CreateTable<InkModel>();
            }
        }

        public static SQLiteConnection GetDbConnection()
        {
            var con = new SQLiteConnection(new SQLitePlatformWinRT(), DbFilePath);
            return con;
        }
    }
}

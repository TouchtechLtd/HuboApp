using System;
using System.IO;
using Hubo.iOS;
using SQLite.Net;
using SQLite.Net.Platform.XamarinIOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_iOS))]

namespace Hubo.iOS
{
    public class SQLite_iOS : ISQLite
    {
        public SQLite_iOS()
        {
        }

        public SQLiteConnection GetConnection()
        {
            SQLitePlatformIOS platform = new SQLitePlatformIOS();
            string sqliteFilename = Configuration.DBname + ".db3";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
            string path = Path.Combine(libraryPath, sqliteFilename);

            return new SQLiteConnection(platform, path);
        }
    }
}
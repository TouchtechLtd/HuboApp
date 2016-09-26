using System.IO;
using Hubo.Windows;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using Windows.Storage;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_WinApp))]

namespace Hubo.Windows
{
    public class SQLite_WinApp : ISQLite
    {
        public SQLite_WinApp()
        {
        }

        public SQLiteConnection GetConnection()
        {
            //download SQLite for Windows Runtime (Windows 8.1) from http://www.sqlite.org/download.html
            //add it and Visual C++ 2013 Runtime Package for Windows as references (Windows 8.1 > Extensions)
            SQLitePlatformWinRT platform = new SQLitePlatformWinRT();
            string sqliteFilename = Configuration.DBname + ".db3";
            string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, sqliteFilename);

            return new SQLiteConnection(platform, path);
        }
    }
}
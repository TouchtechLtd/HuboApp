using System.IO;
using Hubo.UWP;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using Windows.Storage;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_UWP))]

namespace Hubo.UWP
{
    public class SQLite_UWP : ISQLite
    {
        public SQLite_UWP()
        {
        }

        public SQLiteConnection GetConnection()
        {
            //download SQLite for Universal Windows Platform from http://www.sqlite.org/download.html
            //add it and Visual C++ 2013 Runtime Package for Windows as references (Universal Windows > Extensions)
            SQLitePlatformWinRT platform = new SQLitePlatformWinRT();
            string sqliteFilename = Configuration.DBname + ".db3";
            string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, sqliteFilename);

            return new SQLiteConnection(platform, path);
        }    }
}
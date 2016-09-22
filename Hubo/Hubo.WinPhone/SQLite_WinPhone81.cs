using System.IO;
using Hubo.WinPhone;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using Windows.Storage;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_WinPhone81))]

namespace Hubo.WinPhone
{
    public class SQLite_WinPhone81 : ISQLite
    {
        public SQLite_WinPhone81()
        {
        }

        public SQLiteConnection GetConnection()
        {
            //download SQLite for Windows Phone (8.1) from http://www.sqlite.org/download.html
            //add it and Visual C++ 2013 Runtime Package for Windows as references (Windows Phone 8.1 > Extensions)
            SQLitePlatformWinRT platform = new SQLitePlatformWinRT();
            string sqliteFilename = Resources.Resource.DatabaseName + ".db3";
            string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, sqliteFilename);

            return new SQLiteConnection(platform, path);
        }
    }
}
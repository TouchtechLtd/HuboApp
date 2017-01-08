using System;
using System.IO;
using Hubo.Droid;
using SQLite.Net;
using SQLite.Net.Platform.XamarinAndroid;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_Android))]

namespace Hubo.Droid
{
    public class SQLite_Android : ISQLite
    {
        public SQLite_Android()
        {
        }
        
        public SQLiteConnection GetConnection()
        {
            SQLitePlatformAndroidN platform = new SQLitePlatformAndroidN();
            string sqliteFilename = Configuration.DBname + ".db3";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            string path = Path.Combine(documentsPath, sqliteFilename);

            //FOR DEBUG REASON, REMOVE WHEN PUSHING TO USERS
            //path = path.Replace("data/user/0/triotech.hubo.droid/files/", "sdcard/Download/");

            return new SQLiteConnection(platform, path);
        }
    }
}
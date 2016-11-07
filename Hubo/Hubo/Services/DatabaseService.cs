using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using Xamarin.Forms;

namespace Hubo
{
    public class DatabaseService
    {
        public SQLiteConnection db;

        public DatabaseService()
        {
            db = DependencyService.Get<ISQLite>().GetConnection();
            db.CreateTable<UserTable>();
        }

        internal bool CheckLoggedIn()
        {
            List<UserTable> list = db.Query<UserTable>("SELECT * FROM [UserTable]");
            if(list.Count > 0)
            {
                return true;
            }
            return false;
        }

        internal bool Login(UserTable user)
        {
            db.Insert(user);
            return true;
        }

        internal void Logout()
        {
            db.Query<UserTable>("DELETE FROM [UserTable]");
        }

        internal UserTable GetUserInfo()
        {
            List<UserTable> list = db.Query<UserTable>("SELECT * FROM [UserTable]");
            return list[0];
        }

        internal void UpdateUserInfo(UserTable user)
        {
            db.Query<UserTable>("DELETE FROM [UserTable]");
            db.Insert(user);
        }
    }
}

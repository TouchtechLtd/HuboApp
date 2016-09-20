using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo.Services
{
    public static class DatabaseService
    {
        static string dbPath = Configuration.DBPath;
        
        /// <summary>
        /// Create all database tables if they don't exist.
        /// </summary>
        internal static void initDb()
        {
            try
            {
                //TODO initiliase the database
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}

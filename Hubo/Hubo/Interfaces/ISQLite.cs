using SQLite.Net;

namespace Hubo
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
using SQLite.Net.Attributes;

namespace Hubo
{
    class UserTable
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(30)]
        public string FirstName { get; set; }

        [MaxLength(30)]
        public string LastName { get; set; }

        [MaxLength(255)]
        public string Email { get; set; }
    }
}

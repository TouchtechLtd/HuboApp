using SQLite.Net.Attributes;

namespace Hubo
{
    class UserTable
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string User { get; set; }

        [MaxLength(30)]
        public string FirstName { get; set; }

        [MaxLength(30)]
        public string LastName { get; set; }

        public string Email { get; set; }
        public string License { get; set; }
        public string LicenseVersion { get; set; }
        public string Endorsements { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string CompanyEmail { get; set; }
        public string Phone { get; set; }
        public string Token { get; set; }



    }
}

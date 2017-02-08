using SQLite.Net.Attributes;

namespace Hubo
{
    class UserTable
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string UserName { get; set; }

        [MaxLength(30)]
        public string FirstName { get; set; }

        [MaxLength(30)]
        public string LastName { get; set; }

        public string Email { get; set; }
        public string License { get; set; }
        public string LicenseVersion { get; set; }
        public string Endorsements { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int Phone { get; set; }
        public string Token { get; set; }
        public int DriverId { get; set; }

    }
}

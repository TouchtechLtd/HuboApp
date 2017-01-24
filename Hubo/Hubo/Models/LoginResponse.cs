using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hubo
{
    public class LoginResponse
    {
        public long UserId { get; set; }
        public int DriverId { get; set; }
        public string DriverFirstName { get; set; }
        public string DriverSurname { get; set; }
        public string DriverEmail { get; set; }
        public string LicenceNo { get; set; }
        public int LicenceVersion { get; set; }
        public int MobilePh { get; set; }
        public List<CompanyAndVehicles> CompaniesAndVehicle { get; set; }
    }

    public class CompanyAndVehicles
    {
        public CompanyTable Company { get; set; }
        public List<VehicleTable> Vehicles { get; set; }
    }
}

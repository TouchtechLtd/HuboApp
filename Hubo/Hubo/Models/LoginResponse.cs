using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hubo
{
    public class LoginUserResponse
    {
        public long UserId { get; set; }
        public int DriverId { get; set; }
        public string DriverFirstName { get; set; }
        public string DriverSurname { get; set; }
        public string DriverEmail { get; set; }
        public string LicenceNo { get; set; }
        public int LicenceVersion { get; set; }
        public int MobilePh { get; set; }
    }

    public class LoginCompanyResponse
    {
        [JsonProperty(PropertyName = "result")]
        public List<CompanyTable> Companies { get; set; }
    }

    public class LoginVehicleResponse
    {
        [JsonProperty(PropertyName = "result")]
        public List<VehicleResponseModel> Vehicles { get; set; }
    }
}

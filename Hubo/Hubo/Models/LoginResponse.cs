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
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "result")]
        public List<CompanyTable> Companies { get; set; }

        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }

        [JsonProperty(PropertyName = "unAuthorizedRequest")]
        public bool UnAuthorizedRequest { get; set; }
    }

    public class LoginVehicleResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "result")]
        public List<VehicleTable> Vehicles { get; set; }

        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }

        [JsonProperty(PropertyName = "unAuthorizedRequest")]
        public bool UnAuthorizedRequest { get; set; }
    }

    public class LoginShiftResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "result")]
        public List<ShiftResponseModel> Shifts { get; set; }

        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }

        [JsonProperty(PropertyName = "unAuthorizedRequest")]
        public bool UnAuthorizedRequest { get; set; }
    }

    public class LoginDriveResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "result")]
        public List<DriveResponseModel> Drives { get; set; }

        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }

        [JsonProperty(PropertyName = "unAuthorizedRequest")]
        public bool UnAuthorizedRequest { get; set; }
    }

    public class LoginNoteResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "result")]
        public List<NoteResponseModel> Notes { get; set; }

        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }

        [JsonProperty(PropertyName = "unAuthorizedRequest")]
        public bool UnAuthorizedRequest { get; set; }
    }

    public class LoginBreakResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "result")]
        public List<BreakResponseModel> Breaks { get; set; }

        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }

        [JsonProperty(PropertyName = "unAuthorizedRequest")]
        public bool UnAuthorizedRequest { get; set; }
    }
}

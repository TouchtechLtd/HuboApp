using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    class LoginRequestModel
    {
        public string usernameOrEmailAddress { get; set; }
        public string password { get; set; }
    }

    public class LoginResponseModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "driverId")]
        public int DriverId { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "surName")]
        public string Surname { get; set; }

        [JsonProperty(PropertyName = "emailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }

    public class UserResultResponseModel
    {
        [JsonProperty(PropertyName = "driverInfo")]
        public UserResponseModel DriverInfo { get; set; }

        [JsonProperty(PropertyName = "listOfLicences")]
        public List<LicenceResponseModel> ListOfLicences { get; set; }
    }

    public class UserResponseModel
    {
        [JsonProperty(PropertyName = "id")]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "phoneNumber")]
        public int PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "licenceNumber")]
        public string LicenceNumber { get; set; }

        [JsonProperty(PropertyName = "address1")]
        public string Address1 { get; set; }

        [JsonProperty(PropertyName = "address2")]
        public string Address2 { get; set; }

        [JsonProperty(PropertyName = "address3")]
        public string Address3 { get; set; }

        [JsonProperty(PropertyName = "postCode")]
        public string PostCode { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }
    }

    public class LicenceResponseModel
    {
        [JsonProperty(PropertyName = "class")]
        public string Class { get; set; }

        [JsonProperty(PropertyName = "endorsement")]
        public string Endorsement { get; set; }
    }

    public class ShiftResponseModel
    {
        public int Id { get; set; }
        public int DriverId { get; set; }
        public int CompanyId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public double StartLocationLat { get; set; }
        public double StartLocationLong { get; set; }
        public double EndLocationLat { get; set; }
        public double EndLocationLong { get; set; }
        public bool IsActive { get; set; }
    }

    public class VehicleResponseModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "registrationNo")]
        public string Rego { get; set; }

        [JsonProperty(PropertyName = "makeModel")]
        public string MakeModel { get; set; }

        [JsonProperty(PropertyName = "fleetNumber")]
        public string FleetNumber { get; set; }

        [JsonProperty(PropertyName = "companyId")]
        public int CompanyId { get; set; }
    }

    public class DriveResponseModel
    {
        public int Id { get; set; }
        public int ShiftId { get; set; }
        public string StartDrivingDateTime { get; set; }
        public string StopDrivingDateTime { get; set; }
        public int StartHubo { get; set; }
        public int StopHubo { get; set; }
        public bool IsActive { get; set; }
        public int VehicleId { get; set; }
    }

    public class NoteResponseModel
    {
        public int Id { get; set; }
        public int ShiftId { get; set; }
        public int BreakId { get; set; }
        public int DrivingShiftId { get; set; }
        public string NoteText { get; set; }
        public string TimeStamp { get; set; }
    }

    public class BreakResponseModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "shiftId")]
        public int ShiftId { get; set; }

        [JsonProperty(PropertyName = "startBreakDateTime")]
        public string StartBreakDateTime { get; set; }

        [JsonProperty(PropertyName = "stopBreakDateTime")]
        public string StopBreakDateTime { get; set; }

        [JsonProperty(PropertyName = "startBreakLocation")]
        public string StartBreakLocation { get; set; }

        [JsonProperty(PropertyName = "stopBreakLocation")]
        public string StopBreakLocation { get; set; }

        [JsonProperty(PropertyName = "isActive")]
        public bool IsActive { get; set; }
    }
}

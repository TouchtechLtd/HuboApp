﻿using Newtonsoft.Json;
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

    public class UserResponseModel
    {
        [JsonProperty(PropertyName = "id")]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "phoneNumber")]
        public int PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "licenceNumber")]
        public string LicenceNumber { get; set; }

        [JsonProperty(PropertyName = "licenceVersion")]
        public string LicenceVersion { get; set; }

        [JsonProperty(PropertyName = "licenceEndorsements")]
        public string LicenceEndorsements { get; set; }

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

    public class ShiftResponseModel
    {
        public int DriverId { get; set; }
        public int CompanyId { get; set; }
        public int ServerKey { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public double StartLocationLat { get; set; }
        public double StartLocationLong { get; set; }
        public double EndLocationLat { get; set; }
        public double EndLocationLong { get; set; }
        public bool State { get; set; }
    }

    public class DriveResponseModel
    {
        public int Id { get; set; }
        public int ShiftId { get; set; }
        public string TimeStamp { get; set; }
        public bool State { get; set; }
        public int VehicleId { get; set; }
    }

    public class NoteResponseModel
    {
        public int Id { get; set; }
        public int ShiftId { get; set; }
        public int BreakId { get; set; }
        public int DrivingShiftId { get; set; }
        public string NoteText { get; set; }
        public int GeoDataLink { get; set; }
        public string TimeStamp { get; set; }
        public int Hubo { get; set; }
        public bool StandAloneNote { get; set; }
    }

    public class BreakResponseModel
    {
        public int Id { get; set; }
        public int ShiftId { get; set; }
        public int GeoDataId { get; set; }
        public string TimeStamp { get; set; }
        public bool State { get; set; }
    }
}

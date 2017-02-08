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

    class UserRequestModel
    {
        public int id { get; set; }
    }

    class CompanyDetailModel
    {
        public int id { get; set; }
    }

    class VehicleDetailModel
    {
        public int id { get; set; }
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

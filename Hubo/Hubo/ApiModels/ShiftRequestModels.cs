using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    class StartShiftModel
    {
        public int driverId { get; set; }
        public int companyId { get; set; }
        public string startDate { get; set; }
    }

    class EndShiftModel
    {
        public int id { get; set; }
        public string endDate { get; set; }
    }

    class DriveModel
    {
        public int shiftId { get; set; }
        public string timeStamp { get; set; }
        public int vehicleId { get; set; }
    }

    class BreakModel
    {
        public int driveShiftId { get; set; }
        public int geoDataId { get; set; }
        public string timeStamp { get; set; }
    }

    class InsertGeoModel
    {
        public int drivingShiftId { get; set; }
        public string timeStamp { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    class InsertNoteModel
    {
        public int shiftId { get; set; }
        public int breakId { get; set; }
        public int drivingShiftId { get; set; }
        public string noteText { get; set; }
        public int geoDataLink { get; set; }
        public string timeStamp { get; set; }
        public int hubo { get; set; }
    }
}

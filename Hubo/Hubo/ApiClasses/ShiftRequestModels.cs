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
        public double startLocationLat { get; set; }
        public double startLocationLong { get; set; }
    }

    class EndShiftModel
    {
        public int id { get; set; }
        public string endDate { get; set; }
        public double endLocationLat { get; set; }
        public double endLocationLong { get; set; }
    }

    class InsertGeoModel
    {
        public int drivingShiftId { get; set; }
        public string timeStamp { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}

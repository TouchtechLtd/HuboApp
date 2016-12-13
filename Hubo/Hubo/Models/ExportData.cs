using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    class ExportData
    {
        public string shiftStart { get; set; }
        public string shiftEnd { get; set; }
        public string activeShift { get; set; }
        public string vehicleMake { get; set; }
        public string vehicleModel { get; set; }
        public string vehicleRego { get; set; }
        public string vehicleCompany { get; set; }
        public string currentVehicle { get; set; }
        public string huboStart { get; set; }
        public string huboEnd { get; set; }
        public string startLocation { get; set; }
        public string endLocation { get; set; }

        public string breakStart { get; set; }
        public string breakEnd { get; set; }
        public string activeBreak { get; set; }
        public string breakDetails { get; set; }
        public string breakStartHubo { get; set; }
        public string breakEndHubo { get; set; }
        public string breakStartLocation { get; set; }
        public string breakEndLocation { get; set; }

        public string noteTime { get; set; }
        public string noteDetails { get; set; }
        public string noteHubo { get; set; }
        public string noteLocation { get; set; }
    }
}

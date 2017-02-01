using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    class ShiftModel
    {
        public ShiftExpando shift { get; set; }
        public NoteExpando note { get; set; }
    }

    class ShiftExpando
    {
        public int shiftId { get; set; }
        public int driverId { get; set; }
        public int vehicleId { get; set; }
    }

    class NoteExpando
    {
        public string noteText { get; set; }
        public string date { get; set; }
        public int hubo { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}

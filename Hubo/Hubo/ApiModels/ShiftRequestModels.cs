// <copyright file="ShiftRequestModels.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class StartShiftModel
    {
        public int driverId { get; set; }

        public int companyId { get; set; }

        public string startDate { get; set; }

        public double startLocationLat { get; set; }

        public double startLocationLong { get; set; }

        public string startLocation { get; set; }
        public string startNote { get; set; }
    }

    internal class EndShiftModel
    {
        public int id { get; set; }

        public string endDate { get; set; }

        public double endLocationLat { get; set; }

        public double endLocationLong { get; set; }

        public string endLocation { get; set; }
        public string endNote { get; set; }
    }

    internal class DriveStartModel
    {
        public int shiftId { get; set; }

        public int vehicleId { get; set; }

        public string startDrivingDateTime { get; set; }

        public int startHubo { get; set; }
        public string startNote { get; set; }
    }

    internal class DriveEndModel
    {
        public int id { get; set; }

        public string stopDrivingDateTime { get; set; }

        public int stopHubo { get; set; }
        public string endNote { get; set; }
    }

    internal class BreakStartModel
    {
        public int shiftId { get; set; }

        public string startBreakDateTime { get; set; }

        public string startBreakLocation { get; set; }
    }

    internal class BreakEndModel
    {
        public int id { get; set; }

        public string stopBreakDateTime { get; set; }

        public string stopBreakLocation { get; set; }
    }

    internal class InsertGeoModel
    {
        public int drivingShiftId { get; set; }

        public string timeStamp { get; set; }

        public double latitude { get; set; }

        public double longitude { get; set; }
    }

    internal class InsertNoteModel
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

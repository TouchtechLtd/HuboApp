﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    public class Constants
    {
        public const string CONTENT_TYPE = "application/json";

        public const string REST_URL_LOGIN = "/Account/Authenticate";

        public const string REST_URL_ADDVEHICLE = "/Vehicles/registerVehicleAsync";

        public const string REST_URL_ADDSHIFTSTART = "/WorkShift/StartShiftAsync";
        public const string REST_URL_ADDSHIFTEND = "/WorkShift/StopShiftAsync";

        public const string REST_URL_GETUSERDETAILS = "/Account/GetDetailsAsync";
        public const string REST_URL_GETCOMPANYDETAILS = "/Company/getCompanyListAsync";
        public const string REST_URL_GETVEHICLEDETAILS = "/Vehicles/getVehiclesAsync";
        public const string REST_URL_GETSHIFTDETAILS = "/WorkShift/getWorkShiftsAsync";
        public const string REST_URL_GETDRIVEDETAILS = "/DrivingShift/GetDrivingShiftsAsync";
        public const string REST_URL_GETBREAKDETAILS = "/Break/GetBreaksAsync";
        public const string REST_URL_GETNOTEDETAILS = "/Note/GetNotesAsync";

        public const string REST_URL_INSERTGEODATA = "/DrivingShift/InsertGeoPointAsync";
    }
}

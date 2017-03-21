// <copyright file="Constants.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>


namespace Hubo
{
    using Xamarin.Forms;

    public static class Constants
    {
        public const string CONTENT_TYPE = "application/json";

        public const string REST_URL_LOGIN = "/Account/Authenticate";

        public const string REST_URL_ADDVEHICLE = "/Vehicles/registerVehicleAsync";

        public const string REST_URL_ADDSHIFTSTART = "/WorkShift/StartShiftAsync";
        public const string REST_URL_ADDSHIFTEND = "/WorkShift/StopShiftAsync";

        public const string REST_URL_GETUSERDETAILS = "/Account/GetDriverDetailsAsync";
        public const string REST_URL_GETCOMPANYDETAILS = "/Company/getCompanyListAsync";
        public const string REST_URL_GETVEHICLEDETAILS = "/Vehicles/getVehiclesByDriverAsync";

        public const string REST_URL_GETSHIFTDETAILS = "/WorkShift/getWorkShiftsAsync";
        public const string REST_URL_GETDRIVEDETAILS = "/DrivingShift/GetDrivingShiftsAsync";
        public const string REST_URL_GETBREAKDETAILS = "/Break/GetBreaksAsync";
        public const string REST_URL_GETNOTEDETAILS = "/Note/GetNotesAsync";

        public const string REST_URL_INSERTGEODATA = "/DrivingShift/InsertGeoPointAsync";

        public const string REST_URL_ADDDRIVESTART = "/DrivingShift/StartDrivingAsync";
        public const string REST_URL_ADDDRIVEEND = "/DrivingShift/StopDrivingAsync";

        public const string REST_URL_ADDBREAKSTART = "/Break/StartBreakAsync";
        public const string REST_URL_ADDBREAKEND = "/Break/StopBreakAsync";

        public const string REST_URL_INSERTNOTE = "/Note/InsertNoteAsync";

        public const string REST_URL_EXPORTDATA = "";

        public const string REST_URL_REGISTERUSER = "";

        public const string REST_URL_GOOGLEAPI = "https://maps.googleapis.com/maps/api/geocode/json?";

        public const int BREAK_DURATION_TRUCK = 30;

        public static Color RED_COLOR = Color.FromHex("#a2101c");
        public static Color GREEN_COLOR = Color.FromHex("#076409");
        public static Color YELLOW_COLOR = Color.FromHex("#bb9707");
    }
}

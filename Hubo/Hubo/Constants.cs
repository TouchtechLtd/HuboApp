using System;
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

        public const string REST_URL_ADDSHIFTSTART = "/Shift/startShiftAsync";
        public const string REST_URL_ADDSHIFTEND = "/Shift/endShiftAsync";

        public const string REST_URL_GETUSERDETAILS = "/Account/GetDetailsAsync";
        public const string REST_URL_GETCOMPANYDETAILS = "/Company/getCompanyListAsync";
        public const string REST_URL_GETVEHICLEDETAILS = "/Vehicles/getVehiclesAsync";
    }
}

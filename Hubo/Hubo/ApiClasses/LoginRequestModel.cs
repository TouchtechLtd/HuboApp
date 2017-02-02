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

    public class VehicleResponseModel
    {
        public string RegistrationNo { get; set; }
        public string MakeModel { get; set; }
        public int StartingOdometer { get; set; }
        public int CurrentOdometer { get; set; }
        public int CompanyId { get; set; }
    }
}

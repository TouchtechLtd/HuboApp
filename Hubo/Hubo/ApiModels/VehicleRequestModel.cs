﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubo
{
    class VehicleRequestModel
    {
        public string registrationNo { get; set; }
        public string makeModel { get; set; }
        public string fleetNumber { get; set; }
        public int companyId { get; set; }
    }
}
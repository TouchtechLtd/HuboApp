// <copyright file="VehicleRequestModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class VehicleRequestModel
    {
        public string registrationNo { get; set; }

        public string makeModel { get; set; }

        public string fleetNumber { get; set; }

        public int companyId { get; set; }
    }
}

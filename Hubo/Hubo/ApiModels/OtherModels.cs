// <copyright file="OtherModels.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

using Newtonsoft.Json;

namespace Hubo
{
    public class OtherModels
    {
    }

    public class ExportModel
    {
        public int DriverId { get; set; }
    }

    public class RegisterModel
    {
        public string firstName { get; set; }

        public string lastName { get; set; }

        public string email { get; set; }

        public string password { get; set; }
    }

    public class VehicleHuboResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "result")]
        public int Hubo { get; set; }

        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }

        [JsonProperty(PropertyName = "unAuthorizedRequest")]
        public bool UnAuthorizedRequest { get; set; }
    }
}

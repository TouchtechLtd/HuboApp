// <copyright file="OtherResponse.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using Newtonsoft.Json;

    internal class OtherResponse
    {
    }

    internal class ExportResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "result")]
        public int Result { get; set; }

        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }

        [JsonProperty(PropertyName = "unAuthorizedRequest")]
        public bool UnAuthorizedRequest { get; set; }
    }

    internal class UpdateProfileResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "result")]
        public UserTable Result { get; set; }

        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }

        [JsonProperty(PropertyName = "unAuthorizedRequest")]
        public bool UnAuthorizedRequest { get; set; }
    }

    internal class RegisterResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "result")]
        public int Result { get; set; }

        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }

        [JsonProperty(PropertyName = "unAuthorizedRequest")]
        public bool UnAuthorizedRequest { get; set; }
    }
}

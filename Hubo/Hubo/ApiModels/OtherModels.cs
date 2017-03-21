// <copyright file="OtherModels.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    public class OtherModels
    {
    }

    public class ExportModel
    {
        public string email { get; set; }

        public string body { get; set; }
    }

    public class RegisterModel
    {
        public string firstName { get; set; }

        public string lastName { get; set; }

        public string email { get; set; }

        public string password { get; set; }
    }
}

// <copyright file="IEmail.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System.Collections.Generic;

    public interface IEmail
    {
        bool Email(string mailTo, string subject, List<string> filePaths);
    }
}

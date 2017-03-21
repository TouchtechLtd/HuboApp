// <copyright file="ISQLite.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using SQLite.Net;

    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
// <copyright file="INotifyService.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;

    public interface INotifyService
    {
        void PresentNotification(string title, string text, bool endCounter);

        void UpdateNotification(string title, string text, bool endCounter);
    }
}

// <copyright file="INotifyService.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;

    public interface INotifyService
    {
        void LocalNotification(string title, string text, DateTime time, int id);

        void CancelNotification(int notificationId);
    }
}

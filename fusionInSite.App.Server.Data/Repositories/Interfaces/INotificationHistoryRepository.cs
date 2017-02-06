﻿using System;
using FusionInsite.App.Server.Data.Models;

namespace FusionInsite.App.Server.Data.Repositories.Interfaces
{
    public interface INotificationHistoryRepository
    {
        bool IsAlreadySent(PushNotification notification);
        void Add(PushNotification notification);
        void AddLog(int notificationCount, int userCount);
        DateTime GetLastRunTimestamp();
    }
}
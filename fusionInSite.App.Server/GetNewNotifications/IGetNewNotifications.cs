using System;
using System.Collections.Generic;
using FusionInsite.App.Server.Data.Models;

namespace FusionInsite.App.Server.GetNewNotifications
{
    public interface IGetNewNotifications
    {
        List<PushNotification> GetNotifications(DateTime lastRunTimestamp);
    }
}

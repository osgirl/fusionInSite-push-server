using System;
using FusionInsite.App.Server.Data.Models;

namespace FusionInsite.App.Server.Data.Repositories.Interfaces
{
    public interface ISentNotificationRepository
    {
        bool IsAlreadySent(PushNotification notification);
        void Add(PushNotification notification);
    }

    public class SentNotificationRepository : ISentNotificationRepository
    {
        public bool IsAlreadySent(PushNotification notification)
        {
            throw new NotImplementedException();
        }

        public void Add(PushNotification notification)
        {
            throw new NotImplementedException();
        }
    }
}

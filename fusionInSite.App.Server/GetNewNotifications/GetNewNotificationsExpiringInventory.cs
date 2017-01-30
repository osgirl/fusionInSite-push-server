using System;
using System.Collections.Generic;
using FusionInsite.App.Server.Data.Models;
using FusionInsite.App.Server.Data.Repositories.Interfaces;

namespace FusionInsite.App.Server.GetNewNotifications
{
    public class GetNewNotificationsExpiringInventory : IGetNewNotifications
    {
        private readonly IAlertsRepository _alertsRepository;

        public GetNewNotificationsExpiringInventory(IAlertsRepository alertsRepository)
        {
            _alertsRepository = alertsRepository;
        }

        public List<PushNotification> GetNotifications()
        {
            var alerts = _alertsRepository.GetExpiringInventory();

            Console.WriteLine("GetNewNotificationsExpiringInventory");
            return new List<PushNotification>();
        }
    }
}
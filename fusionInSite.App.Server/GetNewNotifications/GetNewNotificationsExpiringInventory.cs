using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<PushNotification> GetNotifications(DateTime lastRunTimestamp)
        {
            var alerts = _alertsRepository.GetExpiringInventory(lastRunTimestamp);
            
            return alerts.Select(a => new PushNotification
            {
                PushNotificationType = PushNotificationType.ExpiringInventory,
                InventoryKey = a.InventoryKey,
                ProtocolId = a.ProtocolId,
                Message = $"Inventory {a.InventoryKey} is expiring on {a.ExpirationDate?.ToShortDateString() ?? ""}"
            }).ToList();
        }
    }
}
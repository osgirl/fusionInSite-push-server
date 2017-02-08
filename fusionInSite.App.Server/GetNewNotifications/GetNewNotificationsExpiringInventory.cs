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
            /* testing: EXECUTE ON TEST DB ONLY
            DELETE FROM tblNotificationInventory
            UPDATE tblPortalInventory SET txtExpirationDate = DATEADD(day, 28, GETDATE()) 
            */
            
            var alerts = _alertsRepository.GetExpiringInventory(lastRunTimestamp, (int)PushNotificationType.ExpiringInventory);
            
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
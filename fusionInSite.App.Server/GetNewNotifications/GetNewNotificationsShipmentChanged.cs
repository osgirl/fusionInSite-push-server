using System;
using System.Collections.Generic;
using System.Linq;
using FusionInsite.App.Server.Data.Models;
using FusionInsite.App.Server.Data.Repositories.Interfaces;

namespace FusionInsite.App.Server.GetNewNotifications
{
    public class GetNewNotificationsShipmentChanged : IGetNewNotifications
    {
        private readonly IAlertsRepository _alertsRepository;

        public GetNewNotificationsShipmentChanged(IAlertsRepository alertsRepository)
        {
            _alertsRepository = alertsRepository;
        }

        public List<PushNotification> GetNotifications(DateTime lastRunTimestamp)
        {
            return new List<PushNotification>
            {
                new PushNotification { PushNotificationType = PushNotificationType.ShipmentStatusChanged, Id = 6, Message = "New Shipment Changed", ProtocolId = 1, StatusId = 1}
            };

            var alerts = _alertsRepository.GetShipmentStatusChanged(lastRunTimestamp);
            Console.WriteLine("GetNewNotificationsShipmentChanged");

            return alerts.Select(a => new PushNotification { PushNotificationType = PushNotificationType.ShipmentStatusChanged}).ToList();
        }
    }
}
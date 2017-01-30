using System;
using System.Collections.Generic;
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

        public List<PushNotification> GetNotifications()
        {
            var alerts = _alertsRepository.GetShipmentStatusChanged();
            Console.WriteLine("GetNewNotificationsShipmentChanged");
            return new List<PushNotification>();
        }
    }
}
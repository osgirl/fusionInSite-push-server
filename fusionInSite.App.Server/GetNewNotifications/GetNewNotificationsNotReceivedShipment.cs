using System;
using System.Collections.Generic;
using FusionInsite.App.Server.Data.Models;
using FusionInsite.App.Server.Data.Repositories.Interfaces;

namespace FusionInsite.App.Server.GetNewNotifications
{
    public class GetNewNotificationsNotReceivedShipment : IGetNewNotifications
    {
        private readonly IAlertsRepository _alertsRepository;

        public GetNewNotificationsNotReceivedShipment(IAlertsRepository alertsRepository)
        {
            _alertsRepository = alertsRepository;
        }

        public List<PushNotification> GetNotifications()
        {
            var alerts = _alertsRepository.GetNotReceivedShipment();
            Console.WriteLine("GetNewNotificationsNotReceivedShipment");
            return new List<PushNotification>();
        }
    }
}
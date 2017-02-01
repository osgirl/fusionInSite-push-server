using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<PushNotification> GetNotifications(DateTime lastRunTimestamp)
        {
            return new List<PushNotification>();


            //var alerts = _alertsRepository.GetNotReceivedShipment(lastRunTimestamp);
            //Console.WriteLine("GetNewNotificationsNotReceivedShipment");
            //return alerts.Select(a => new PushNotification { PushNotificationType = PushNotificationType.NotReceivedShipment }).ToList();
        }
    }
}
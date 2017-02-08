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
            /* testing: EXECUTE ON TEST DB ONLY
            UPDATE tblPortalShipments SET txtShipmentStatusID = (SELECT MAX(txtShipmentStatusID) FROM tblPortalShipments) + 1, txtCarrierStatusDate = GETDATE()
            */

            var alerts = _alertsRepository.GetShipmentStatusChanged(lastRunTimestamp);

            return alerts.Select(a => new PushNotification
            {
                PushNotificationType = PushNotificationType.ShipmentStatusChanged,
                ShipmentKey = a.ShipmentKey,
                ProtocolId = a.ProtocolId,
                StatusId = a.ShipmentStatusId,
                Message = $"Shipment {a.ShipmentKey} status has changed to {a.ShipmentStatus}"
            }).ToList();
        }
    }
}
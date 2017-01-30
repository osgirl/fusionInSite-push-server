using System.Collections.Generic;

namespace Catalent.App.Server.Data
{
    public interface IShipmentSubscriptionRepository
    {
        void Subscribe(ShipmentSubscription shipmentSubscription);
        void Unsubscribe(ShipmentSubscription shipmentSubscription);
        List<ShipmentSubscription> GetActiveSubscriptions();
    }
}
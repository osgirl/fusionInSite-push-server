using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalent.App.Server.Data
{
    public class ShipmentSubscriptionRepository : IShipmentSubscriptionRepository
    {
        public void Subscribe(ShipmentSubscription shipmentSubscription)
        {
            
        }

        public void Unsubscribe(ShipmentSubscription shipmentSubscription)
        {
            
        }

        public List<ShipmentSubscription> GetActiveSubscriptions()
        {
            return new List<ShipmentSubscription>();
        }
    }
}

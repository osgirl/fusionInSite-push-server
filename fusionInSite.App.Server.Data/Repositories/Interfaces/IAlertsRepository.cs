using System;
using System.Collections.Generic;
using System.Data;

namespace FusionInsite.App.Server.Data.Repositories.Interfaces
{
    public interface IAlertsRepository
    {
        List<ExpiringInventoryItem> GetExpiringInventory(DateTime lastRunTimestamp, int notificationTypeID);
        //List<DataRow> GetNotReceivedShipment(DateTime lastRunTimestamp); // Cannot use until data is consistent
        List<ShipmentStatusChangedItem> GetShipmentStatusChanged(DateTime lastRunTimestamp);
    }
    
}
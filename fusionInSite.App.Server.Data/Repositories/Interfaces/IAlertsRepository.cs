using System;
using System.Collections.Generic;
using System.Data;
using FusionInsite.App.Server.Data.Models;

namespace FusionInsite.App.Server.Data.Repositories.Interfaces
{
    public interface IAlertsRepository
    {
        List<ExpiringInventoryItem> GetExpiringInventory(DateTime lastRunTimestamp, int notificationTypeId);
        //List<DataRow> GetNotReceivedShipment(DateTime lastRunTimestamp); // Cannot use until data is consistent
        List<ShipmentStatusChangedItem> GetShipmentStatusChanged(DateTime lastRunTimestamp, int notificationTypeId);
    }
    
}
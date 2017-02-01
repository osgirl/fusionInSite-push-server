using System;
using System.Collections.Generic;
using System.Data;

namespace FusionInsite.App.Server.Data.Repositories.Interfaces
{
    public interface IAlertsRepository
    {
        List<DataRow> GetExpiringInventory(DateTime lastRunTimestamp);
        List<DataRow> GetNotReceivedShipment(DateTime lastRunTimestamp);
        List<DataRow> GetShipmentStatusChanged(DateTime lastRunTimestamp);
    }
    
}
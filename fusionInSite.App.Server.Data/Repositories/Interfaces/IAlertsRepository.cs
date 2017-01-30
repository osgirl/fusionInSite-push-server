using System.Collections.Generic;
using System.Data;

namespace FusionInsite.App.Server.Data.Repositories.Interfaces
{
    public interface IAlertsRepository
    {
        List<DataRow> GetExpiringInventory();
        List<DataRow> GetNotReceivedShipment();
        List<DataRow> GetShipmentStatusChanged();
    }
    
}
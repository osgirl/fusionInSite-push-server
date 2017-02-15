using System.Text;
using System.Threading.Tasks;

namespace FusionInsite.App.Server.Data.Models
{
    public class PushNotification
    {
        public int ShipmentKey { get; set; }
        public int InventoryKey { get; set; }
        public int? ProtocolId { get; set; }
        public PushNotificationType PushNotificationType { get; set; }
        public int? StatusId { get; set; }
        public string Message { get; set; }
        public string AptuitShipmentId { get; set; }
        public string AptuitLotBatchNo { get; set; }
        public string ItemID { get; set; }
    }
}

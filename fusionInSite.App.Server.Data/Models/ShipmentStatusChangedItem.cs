using System;

namespace FusionInsite.App.Server.Data.Models
{
    public class ShipmentStatusChangedItem
    {
        public int ShipmentKey { get; set; }
        public int? ProtocolId { get; set; }
        public string AptuitShipmentId { get; set; }
        public string ShipmentStatus { get; set; }
        public string CarrierStatus { get; set; }
        public DateTime? ShipmentEnteredDate { get; set; }
        public DateTime? ActualShipDate { get; set; }
        public DateTime? CarrierStatusDate { get; set; }
        public int? ShipmentStatusId { get; set; }
        public string AptuitSiteId { get; set; }
    }
}
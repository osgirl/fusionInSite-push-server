using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FusionInsite.App.Server.Data.Repositories.Interfaces;

namespace FusionInsite.App.Server.Data.Repositories
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
        public int ShipmentStatusID { get; set; }
    }

    public class AlertsRepository : IAlertsRepository
    {
        public List<ShipmentStatusChangedItem> GetShipmentStatusChanged(DateTime lastRunTimestamp)
        {
            using (var conn = new SqlConnection{ConnectionString = ConfigurationManager.AppSettings["ConnectionString"]})
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT ShipmentKey, txtProtocolID, txtAptuitShipmentID, txtShipmentStatus, txtShipmentStatusID, txtCarrierStatus, txtShipmentEnteredDate, txtActualShipDate, txtCarrierStatusDate 
                                           FROM tblPortalShipments WITH(NOLOCK)
                                           WHERE (txtCarrierStatusDate > @LastRunDate OR txtLastUpdate > @LastRunDate)
                                           AND TxtShipmentStatus IN('Shipped', 'Delivered')", conn) { CommandType = CommandType.Text };
                cmd.Parameters.Add(new SqlParameter("@LastRunDate", lastRunTimestamp));
                var rdr = cmd.ExecuteReader();
                var shipmentStatusChangedItems = new List<ShipmentStatusChangedItem>();
                while (rdr.Read())
                {
                    shipmentStatusChangedItems.Add(new ShipmentStatusChangedItem
                    {
                        ShipmentKey = (int)rdr["ShipmentKey"],
                        ProtocolId = (int?)rdr["txtProtocolID"],
                        AptuitShipmentId = rdr["txtAptuitShipmentID"].ToString(),
                        ShipmentStatus = rdr["txtShipmentStatus"].ToString(),
                        CarrierStatus = rdr["txtCarrierStatus"].ToString(),
                        ShipmentStatusID = (int) rdr["txtShipmentStatusID"],
                        ShipmentEnteredDate = (DateTime?)rdr["txtShipmentEnteredDate"],
                        ActualShipDate = (DateTime?)rdr["txtActualShipDate"],
                        CarrierStatusDate = (DateTime?)rdr["txtCarrierStatusDate"],
                    });
                }
                return shipmentStatusChangedItems;
            }
        }


        /// <summary>
        /// This should not be used until data is consistently provided from couriers. Also is not tested or fully defined. 
        /// </summary>
        /// <param name="lastRunTimestamp"></param>
        /// <returns></returns>
        //public List<DataRow> GetNotReceivedShipment(DateTime lastRunTimestamp)
        //{

        //    using (var conn = new SqlConnection { ConnectionString = ConfigurationManager.AppSettings["ConnectionString"] })
        //    {
        //        conn.Open();
        //        var cmd = new SqlCommand(@"SELECT txtProtocolID, txtAptuitShipmentID, txtShipmentStatus, txtCarrierStatus, txtShipmentEnteredDate,txtPlannedShipDate,txtActualShipDate, txtCarrierStatusDate
        //                                   FROM tblPortalShipments WITH(NOLOCK)
        //                                   WHERE  txtActualShipDate >= @From AND txtActualShipDate < @To
        //                                   AND 	  TxtShipmentStatus IN ('Shipped')", conn) { CommandType = CommandType.Text };
        //        cmd.Parameters.Add(new SqlParameter("@From", lastRunTimestamp.AddDays(-5)));
        //        cmd.Parameters.Add(new SqlParameter("@To", DateTime.Now.AddDays(-5)));
        //        var rdr = cmd.ExecuteReader();
        //        var dateRange = new List<DataRow>();
        //        while (rdr.Read())
        //        {
        //            //  dateRange.Add(rdr["DateRangeName"].ToString());
        //        }
        //        return dateRange;
        //    }
        //}

        // TODO: ************************* AND txtStatusID IS NOT xxxxxxxxxxxxxxxxxxxxxxxxxxx
        public List<ExpiringInventoryItem> GetExpiringInventory(DateTime lastRunTimestamp)
        {
            using (var conn = new SqlConnection { ConnectionString = ConfigurationManager.AppSettings["ConnectionString"] })
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT InventoryKey, txtProtocolID, txtExpirationDate 
                                           FROM tblPortalInventory WITH(NOLOCK)
                                           WHERE  txtExpirationDate >= @From AND txtExpirationDate < @to AND txtStatus <> 'Quarantine'", conn) { CommandType = CommandType.Text };
                cmd.Parameters.Add(new SqlParameter("@From", lastRunTimestamp.AddDays(24))); 
                cmd.Parameters.Add(new SqlParameter("@To", DateTime.Now.AddDays(30)));
                var rdr = cmd.ExecuteReader();
                var expiringInventoryItems = new List<ExpiringInventoryItem>();
                while (rdr.Read())
                {
                    expiringInventoryItems.Add(new ExpiringInventoryItem
                    {
                        InventoryKey = (int) rdr["InventoryKey"],
                        ProtocolId = (int?) rdr["txtProtocolID"],
                        ExpirationDate = (DateTime?) rdr["txtExpirationDate"],
                    });
                }
                return expiringInventoryItems;
            }
        }
        
    }

    public class ExpiringInventoryItem
    {
        public int InventoryKey { get; set; }
        public int? ProtocolId { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}

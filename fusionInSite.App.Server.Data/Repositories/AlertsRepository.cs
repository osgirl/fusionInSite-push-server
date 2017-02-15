using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FusionInsite.App.Server.Data.Models;
using FusionInsite.App.Server.Data.Repositories.Interfaces;

namespace FusionInsite.App.Server.Data.Repositories
{
    public class AlertsRepository : IAlertsRepository
    {
        public List<ShipmentStatusChangedItem> GetShipmentStatusChanged(DateTime lastRunTimestamp, int notificationTypeId)
        {
            using (var conn = new SqlConnection{ConnectionString = ConfigurationManager.AppSettings["ConnectionString"]})
            {
                conn.Open(); // Composite ProtocolID and ShipmentID
                // TODO: Look at index on txtCarrierStatusDate
                var cmd = new SqlCommand("SELECT S.ShipmentKey, S.txtProtocolID, S.txtAptuitShipmentID, S.txtAptuitSiteID, S.txtShipmentStatus, S.txtShipmentStatusID, S.txtCarrierStatus, S.txtShipmentEnteredDate, S.txtActualShipDate, S.txtCarrierStatusDate " +
                                         "FROM tblPortalShipments S WITH(NOLOCK) " +
                                         "LEFT JOIN tblNotificationShipment NS WITH(NOLOCK) ON S.txtProtocolID = NS.ProtocolID AND S.txtAptuitShipmentID = NS.txtAptuitShipmentID AND NS.NotificationTypeID = @NotificationTypeID AND NS.txtShipmentStatusID = S.txtShipmentStatusID " +
                                         "WHERE (S.txtCarrierStatusDate > @LastRunDate OR S.txtLastUpdate > @LastRunDate) " +
                                         "AND NS.NotificationID IS NULL " +
                                         "AND S.TxtShipmentStatus IN('Shipped', 'Delivered') " +
                                         "AND EXISTS (SELECT 1 FROM tblNotificationUserProtocolSubscription SUB WHERE S.txtProtocolID = SUB.ProtocolID AND SUB.Area = 'Shipments')", conn) { CommandType = CommandType.Text };
                cmd.Parameters.Add(new SqlParameter("@LastRunDate", lastRunTimestamp));
                cmd.Parameters.Add(new SqlParameter("@NotificationTypeID", notificationTypeId));
                var rdr = cmd.ExecuteReader();
                var shipmentStatusChangedItems = new List<ShipmentStatusChangedItem>();
                while (rdr.Read())
                {
                    shipmentStatusChangedItems.Add(new ShipmentStatusChangedItem
                    {
                        ShipmentKey = (int)rdr["ShipmentKey"],
                        ProtocolId = (int?)DbNullToNull(rdr["txtProtocolID"]),
                        AptuitShipmentId = rdr["txtAptuitShipmentID"].ToString(),
                        AptuitSiteId = rdr["txtAptuitSiteID"].ToString(),
                        ShipmentStatus = rdr["txtShipmentStatus"].ToString(),
                        CarrierStatus = rdr["txtCarrierStatus"].ToString(),
                        ShipmentStatusId = (int?) DbNullToNull(rdr["txtShipmentStatusID"]),
                        ShipmentEnteredDate = (DateTime?) DbNullToNull(rdr["txtShipmentEnteredDate"]),
                        ActualShipDate = (DateTime?)DbNullToNull(rdr["txtActualShipDate"]),
                        CarrierStatusDate = (DateTime?)DbNullToNull(rdr["txtCarrierStatusDate"]),
                    });
                }
                return shipmentStatusChangedItems;
            }
        }

        private static object DbNullToNull(object value)
        {
            return value == null || value == DBNull.Value ? null : value;
        }


        /// <summary>
        /// This should not be used until data is consistently provided from couriers. Also is not tested or fully defined. 
        /// </summary>
        /// <param name="lastRunTimestamp"></param>
        /// <param name="notificationTypeId"></param>
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
        public List<ExpiringInventoryItem> GetExpiringInventory(DateTime lastRunTimestamp, int notificationTypeId)
        {
            using (var conn = new SqlConnection { ConnectionString = ConfigurationManager.AppSettings["ConnectionString"] })
            {
                conn.Open(); // Composite ProtocolID and AptuitLotBatchNo and ItemID
                var cmd = new SqlCommand("SELECT I.InventoryKey, I.txtProtocolID, I.txtExpirationDate, I.txtAptuitLotBatchNo, I.txtItemID " +
                                         "FROM tblPortalInventory I WITH(NOLOCK) " +
                                         "LEFT JOIN tblNotificationInventory NI WITH(NOLOCK) ON I.txtProtocolID = NI.ProtocolID AND I.txtAptuitLotBatchNo = NI.txtAptuitLotBatchNo AND I.txtItemID = NI.txtItemID AND NI.NotificationTypeID = @NotificationTypeID " +
                                         "WHERE I.txtExpirationDate >= @From AND I.txtExpirationDate < @to AND I.txtStatus <> 'Quarantine' " +
                                         "AND NI.NotificationID IS NULL " +
                                         "AND EXISTS (SELECT 1 FROM tblNotificationUserProtocolSubscription SUB WHERE I.txtProtocolID = SUB.ProtocolID AND SUB.Area = 'Inventory')", conn) { CommandType = CommandType.Text };
                var searchFrom = new DateTime(Math.Max(lastRunTimestamp.AddDays(30).Ticks, DateTime.Now.AddDays(23).Ticks));
                cmd.Parameters.Add(new SqlParameter("@From", searchFrom));
                cmd.Parameters.Add(new SqlParameter("@To", DateTime.Now.AddDays(30)));
                cmd.Parameters.Add(new SqlParameter("@NotificationTypeID", notificationTypeId));
                var rdr = cmd.ExecuteReader();
                var expiringInventoryItems = new List<ExpiringInventoryItem>();
                while (rdr.Read())
                {
                    expiringInventoryItems.Add(new ExpiringInventoryItem
                    {
                        InventoryKey = (int) rdr["InventoryKey"],
                        ProtocolId = (int?)DbNullToNull(rdr["txtProtocolID"]),
                        ExpirationDate = (DateTime?)DbNullToNull(rdr["txtExpirationDate"]),
                        AptuitLotBatchNo = rdr["txtAptuitLotBatchNo"].ToString(),
                        ItemID = rdr["txtItemID"].ToString(),
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
        public string AptuitLotBatchNo { get; set; }
        public string ItemID { get; set; }
    }
}

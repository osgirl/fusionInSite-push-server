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
    public class AlertsRepository : IAlertsRepository
    {
        public List<DataRow> GetShipmentStatusChanged(DateTime lastRunTimestamp)
        {
            using (var conn = new SqlConnection{ConnectionString = ConfigurationManager.AppSettings["ConnectionString"]})
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT ShipmentKey, txtProtocolID, txtAptuitShipmentID, txtShipmentStatus, txtCarrierStatus, txtShipmentEnteredDate, txtPlannedShipDate, txtActualShipDate, txtCarrierStatusDate 
                                           FROM tblPortalShipments WITH(NOLOCK)
                                           WHERE (txtCarrierStatusDate > @LastRunDate OR txtLastUpdate > @LastRunDate)
                                           AND TxtShipmentStatus IN('Shipped', 'Delivered')", conn) { CommandType = CommandType.Text };
                cmd.Parameters.Add(new SqlParameter("@LastRunDate", lastRunTimestamp));
                var rdr = cmd.ExecuteReader();
                var dateRange = new List<DataRow>();
                while (rdr.Read())
                {
                  //  dateRange.Add(rdr["DateRangeName"].ToString());
                }
                return dateRange;
            }
        }


        /// <summary>
        /// This should not be used until data is consistently provided from couriers.
        /// </summary>
        /// <param name="lastRunTimestamp"></param>
        /// <returns></returns>
        public List<DataRow> GetNotReceivedShipment(DateTime lastRunTimestamp)
        {

            using (var conn = new SqlConnection { ConnectionString = ConfigurationManager.AppSettings["ConnectionString"] })
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT txtProtocolID, txtAptuitShipmentID, txtShipmentStatus, txtCarrierStatus, txtShipmentEnteredDate,txtPlannedShipDate,txtActualShipDate, txtCarrierStatusDate
                                           FROM tblPortalShipments WITH(NOLOCK)
                                           WHERE  txtActualShipDate >= @From AND txtActualShipDate < @To
                                           AND 	  TxtShipmentStatus IN ('Shipped')", conn) { CommandType = CommandType.Text };
                cmd.Parameters.Add(new SqlParameter("@From", lastRunTimestamp.AddDays(-5)));
                cmd.Parameters.Add(new SqlParameter("@To", DateTime.Now.AddDays(-5)));
                var rdr = cmd.ExecuteReader();
                var dateRange = new List<DataRow>();
                while (rdr.Read())
                {
                    //  dateRange.Add(rdr["DateRangeName"].ToString());
                }
                return dateRange;
            }
        }

        public List<DataRow> GetExpiringInventory(DateTime lastRunTimestamp)
        {
            using (var conn = new SqlConnection { ConnectionString = ConfigurationManager.AppSettings["ConnectionString"] })
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT txtProtocolID, txtExpirationDate, txtStatusID 
                                           FROM tblPortalInventory WITH(NOLOCK)
                                           WHERE  txtExpirationDate >= @From AND txtExpirationDate < @to", conn) { CommandType = CommandType.Text };
                cmd.Parameters.Add(new SqlParameter("@From", lastRunTimestamp.AddDays(30))); 
                cmd.Parameters.Add(new SqlParameter("@To", DateTime.Now.AddDays(30)));
                var rdr = cmd.ExecuteReader();
                var dateRange = new List<DataRow>();
                while (rdr.Read())
                {
                    //  dateRange.Add(rdr["DateRangeName"].ToString());
                }
                return dateRange;
            }
        }
    }
}

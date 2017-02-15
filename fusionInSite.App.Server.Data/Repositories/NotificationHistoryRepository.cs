using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using FusionInsite.App.Server.Data.Models;
using FusionInsite.App.Server.Data.Repositories.Interfaces;

namespace FusionInsite.App.Server.Data.Repositories
{
    public class NotificationHistoryRepository : INotificationHistoryRepository
    {
        public bool IsAlreadySent(PushNotification notification)
        {
            using (var conn = new SqlConnection { ConnectionString = ConfigurationManager.AppSettings["ConnectionString"] })
            {
                conn.Open();
                switch (notification.PushNotificationType)
                {
                    case PushNotificationType.ShipmentStatusChanged:
                    {
                        var cmd = new SqlCommand(@"SELECT 1 FROM tblNotificationShipment WITH(NOLOCK) WHERE ShipmentKey = @ShipmentKey AND txtShipmentStatusID = @txtShipmentStatusID AND NotificationTypeID = @NotificationTypeID", conn);
                        cmd.Parameters.Add(new SqlParameter("@ShipmentKey", notification.ShipmentKey));
                        cmd.Parameters.Add(new SqlParameter("@txtShipmentStatusID", (object)notification.StatusId ?? DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@NotificationTypeID", notification.PushNotificationType));
                        var result = cmd.ExecuteScalar();
                        return result != null && result != DBNull.Value;
                    }
                    case PushNotificationType.ExpiringInventory:
                    {
                        var cmd = new SqlCommand(@"SELECT 1 FROM tblNotificationInventory WITH(NOLOCK) WHERE InventoryKey = @InventoryKey AND NotificationTypeID = @NotificationTypeID", conn);
                        cmd.Parameters.Add(new SqlParameter("@InventoryKey", notification.InventoryKey));
                        cmd.Parameters.Add(new SqlParameter("@NotificationTypeID", notification.PushNotificationType));
                        var result = cmd.ExecuteScalar();
                        return result != null && result != DBNull.Value;
                    }
                    default:
                        throw new NotImplementedException();
                }

            }
        }

        public int Add(UserMessage userMessage)
        {
            using (var conn = new SqlConnection { ConnectionString = ConfigurationManager.AppSettings["ConnectionString"] })
            {
                conn.Open();
                var notificationId = int.Parse(new SqlCommand(@"INSERT INTO tblNotification DEFAULT VALUES; SELECT SCOPE_IDENTITY()", conn).ExecuteScalar().ToString());

                foreach (var notification in userMessage.PushNotifications)
                {
                    switch (notification.PushNotificationType)
                    {
                        case PushNotificationType.ShipmentStatusChanged:
                            {
                                var cmd = new SqlCommand(@"INSERT INTO tblNotificationShipment (NotificationID, ProtocolID, txtAptuitShipmentID, txtShipmentStatusID, NotificationTypeID)
                                                   VALUES (@NotificationID, @ProtocolID, @AptuitShipmentId, @txtShipmentStatusID, @NotificationTypeID)", conn);
                                cmd.Parameters.Add(new SqlParameter("@NotificationID", notificationId));
                                cmd.Parameters.Add(new SqlParameter("@ProtocolId", notification.ProtocolId));
                                cmd.Parameters.Add(new SqlParameter("@AptuitShipmentId", notification.AptuitShipmentId));
                                cmd.Parameters.Add(new SqlParameter("@txtShipmentStatusID", (object)notification.StatusId ?? DBNull.Value));
                                cmd.Parameters.Add(new SqlParameter("@NotificationTypeID", notification.PushNotificationType));
                                cmd.ExecuteNonQuery();
                            }
                            break;
                        case PushNotificationType.ExpiringInventory:
                            {
                                var cmd =
                                    new SqlCommand(@"INSERT INTO tblNotificationInventory (NotificationID, ProtocolID, txtAptuitLotBatchNo, txtItemID, NotificationTypeID)
                                             VALUES (@NotificationID, @ProtocolID, @txtAptuitLotBatchNo, @txtItemID, @NotificationTypeID)", conn)
                                    {
                                        CommandType = CommandType.Text
                                    };
                                cmd.Parameters.Add(new SqlParameter("@NotificationID", notificationId));
                                cmd.Parameters.Add(new SqlParameter("@ProtocolID", notification.ProtocolId));
                                cmd.Parameters.Add(new SqlParameter("@txtAptuitLotBatchNo", notification.AptuitLotBatchNo));
                                cmd.Parameters.Add(new SqlParameter("@txtItemID", notification.ItemID));
                                cmd.Parameters.Add(new SqlParameter("@NotificationTypeID", notification.PushNotificationType));

                                cmd.ExecuteNonQuery();
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                }

                return notificationId;
            }
        }

        public void AddLog(int notificationCount, int userCount)
        {
            using (var conn = new SqlConnection { ConnectionString = ConfigurationManager.AppSettings["ConnectionString"] })
            {
                conn.Open();
         
                var cmd = new SqlCommand(@"INSERT INTO tblNotificationLog (NotificationCount, UserCount)
                                            VALUES (@NotificationCount, @UserCount)", conn);
                cmd.Parameters.Add(new SqlParameter("@NotificationCount", notificationCount));
                cmd.Parameters.Add(new SqlParameter("@UserCount", userCount));
                cmd.ExecuteNonQuery();
            }
        }

        public DateTime GetLastRunTimestamp()
        {
            using (var conn = new SqlConnection { ConnectionString = ConfigurationManager.AppSettings["ConnectionString"] })
            {
                conn.Open();
         
                var cmd = new SqlCommand(@"SELECT MAX(SentTimestamp) FROM tblNotificationLog", conn);
                var sentTimestamp = cmd.ExecuteScalar();
                return sentTimestamp == null || sentTimestamp == DBNull.Value
                    ? SqlDateTime.MinValue.Value
                    : (DateTime) sentTimestamp;
            }
        }
    }
}
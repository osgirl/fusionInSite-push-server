using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
                        cmd.Parameters.Add(new SqlParameter("@ShipmentKey", notification.Id));
                        cmd.Parameters.Add(new SqlParameter("@txtShipmentStatusID", notification.StatusId));
                        cmd.Parameters.Add(new SqlParameter("@NotificationTypeID", notification.PushNotificationType));
                        var result = cmd.ExecuteScalar();
                        return result != null && result != DBNull.Value;
                    }
                    case PushNotificationType.ExpiringInventory:
                    {
                        var cmd = new SqlCommand(@"SELECT 1 FROM tblNotificationInventory WITH(NOLOCK) WHERE ShipmentKey = @ShipmentKey AND NotificationTypeID = @NotificationTypeID", conn);
                        cmd.Parameters.Add(new SqlParameter("@ShipmentKey", notification.Id));
                        cmd.Parameters.Add(new SqlParameter("@NotificationTypeID", notification.PushNotificationType));
                        var result = cmd.ExecuteScalar();
                        return result != null && result != DBNull.Value;
                    }
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public void Add(PushNotification notification)
        {
            using (var conn = new SqlConnection { ConnectionString = ConfigurationManager.AppSettings["ConnectionString"] })
            {
                conn.Open();
                var notificationId = new SqlCommand(@"INSERT INTO tblNotification DEFAULT VALUES; SELECT SCOPE_IDENTITY()", conn).ExecuteScalar().ToString();

                switch (notification.PushNotificationType)
                {
                    case PushNotificationType.ShipmentStatusChanged:
                    {
                        var cmd = new SqlCommand(@"INSERT INTO tblNotificationShipment (NotificationID, ShipmentKey, txtShipmentStatusID, NotificationTypeID)
                                               VALUES (@NotificationID, @ShipmentKey, @txtShipmentStatusID, @NotificationTypeID)", conn);
                        cmd.Parameters.Add(new SqlParameter("@NotificationID", notificationId));
                        cmd.Parameters.Add(new SqlParameter("@ShipmentKey", notification.Id));
                        cmd.Parameters.Add(new SqlParameter("@txtShipmentStatusID", notification.StatusId));
                        cmd.Parameters.Add(new SqlParameter("@NotificationTypeID", notification.PushNotificationType));
                        cmd.ExecuteNonQuery();
                    }
                        break;
                    case PushNotificationType.ExpiringInventory:
                    {
                        var cmd =
                            new SqlCommand(@"INSERT INTO tblNotificationInventory (NotificationID, ShipmentKey, NotificationTypeID)
                                               VALUES (@NotificationID, @ShipmentKey,  @NotificationTypeID)", conn)
                            {
                                CommandType = CommandType.Text
                            };
                        cmd.Parameters.Add(new SqlParameter("@NotificationID", notificationId));
                        cmd.Parameters.Add(new SqlParameter("@ShipmentKey", notification.Id));
                        cmd.Parameters.Add(new SqlParameter("@NotificationTypeID", notification.PushNotificationType));
                        cmd.ExecuteNonQuery();
                    }
                        break;
                    default:
                        throw new NotImplementedException();
                }
                
            }
        }
    }
}
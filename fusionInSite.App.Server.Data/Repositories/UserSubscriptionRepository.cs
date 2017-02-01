using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using FusionInsite.App.Server.Data.Models;
using FusionInsite.App.Server.Data.Repositories.Interfaces;

namespace FusionInsite.App.Server.Data.Repositories
{
    public class UserSubscriptionRepository : IUserSubscriptionRepository
    {
        public List<string> GetUserTokensSubscribedToProtocol(int protocolId, PushNotificationType notificationTypeId)
        {
            using (var conn = new SqlConnection { ConnectionString = ConfigurationManager.AppSettings["ConnectionString"] })
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT TK.Token FROM tblNotificationUserProtocolSubscription PS WITH(NOLOCK) 
                                           JOIN tblNotificationTypeUserSubscription TS WITH(NOLOCK)  ON PS.UserID = TS.UserID AND TS.NotificationTypeID = @NotificationTypeID
                                           JOIN tblNotificationToken TK WITH(NOLOCK) ON TS.UserID = TK.UserID
                                           WHERE PS.ProtocolID = @ProtocolID", conn);
                cmd.Parameters.Add(new SqlParameter("@ProtocolID", protocolId));
                cmd.Parameters.Add(new SqlParameter("@NotificationTypeID", (int)notificationTypeId));
                var rdr = cmd.ExecuteReader();
                var tokens = new List<string>();
                while (rdr.Read())
                {
                  tokens.Add(rdr["Token"].ToString());
                }
                return tokens;
            }
        }
    }
}
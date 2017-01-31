using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using FusionInsite.App.Server.Data.Repositories.Interfaces;

namespace FusionInsite.App.Server.Data.Repositories
{
    public class UserSubscriptionRepository : IUserSubscriptionRepository
    {
        public List<string> GetUsersSubscribedToProtocol(int protocolId)
        {
            using (var conn = new SqlConnection { ConnectionString = ConfigurationManager.AppSettings["ConnectionString"] })
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT UserID FROM tblNotificationUserProtocolSubscription WITH(NOLOCK) WHERE ProtocolID = @ProtocolID", conn);
                cmd.Parameters.Add(new SqlParameter("@ProtocolID", protocolId));
                var rdr = cmd.ExecuteReader();
                var users = new List<string>();
                while (rdr.Read())
                {
                  users.Add(rdr["UserID"].ToString());
                }
                return users;
            }
        }
    }
}
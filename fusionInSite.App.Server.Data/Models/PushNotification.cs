using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionInsite.App.Server.Data.Models
{
    public class UserPushNotification
    {
        public UserPushNotification(PushNotification pushNotification, string token)
        {
            PushNotification = pushNotification;
            Token = token;
        }

        public PushNotification PushNotification { get; set; }

        public string Token { get; set; }

        public UserPushNotification WithUserToken(string token)
        {
            Token = token;
            return this;
        }
    }

    public class PushNotification
    {
        public int ShipmentKey { get; set; }
        public int InventoryKey { get; set; }
        public int? ProtocolId { get; set; }
        public PushNotificationType PushNotificationType { get; set; }
        public int? StatusId { get; set; }
        public string Message { get; set; }
    }

    public class UserMessage
    {
        public List<string> Token { get; set; }

        public List<PushNotification> PushNotifications { get; set; }
        public string Message { get; set; }

        public UserMessage()
        {
            PushNotifications = new List<PushNotification>();
        }
    }

}

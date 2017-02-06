using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionInsite.App.Server.Data.Models
{
    public class UserPushNotification : PushNotification
    {
        public UserPushNotification(PushNotification pushNotification)
        {
            ShipmentKey = pushNotification.ShipmentKey;
            InventoryKey = pushNotification.InventoryKey;
            PushNotificationType = pushNotification.PushNotificationType;
            ProtocolId = pushNotification.ProtocolId;
            Message = pushNotification.Message;
        }

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
        public List<int> InventoryKeys { get; set; }
        public List<int> ShipmentKeys { get; set; }
        public string Message { get; set; }

        public UserMessage()
        {
            InventoryKeys= new List<int>();
            ShipmentKeys = new List<int>();
        }
        
    }
}

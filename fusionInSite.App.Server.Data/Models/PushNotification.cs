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
            Id = pushNotification.Id;
            PushNotificationType = pushNotification.PushNotificationType;
            ProtocolId = pushNotification.ProtocolId;
            Message = pushNotification.Message;
        }

        public string User { get; set; }

        public UserPushNotification WithUser(string user)
        {
            User = user;
            return this;
        }
    }

    public class PushNotification
    {
        public int Id { get; set; }
        public string ProtocolId { get; set; }
        public PushNotificationType PushNotificationType { get; set; }
        public string Message { get; set; }
    }

    public class UserMessage
    {
        public string User { get; set; }
        public string Message { get; set; }
    }
}

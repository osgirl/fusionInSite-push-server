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

        public string Token { get; set; }

        public UserPushNotification WithUserToken(string token)
        {
            Token = token;
            return this;
        }
    }

    public class PushNotification
    {
        public int Id { get; set; }
        public int ProtocolId { get; set; }
        public PushNotificationType PushNotificationType { get; set; }
        public int StatusId { get; set; }
        public string Message { get; set; }
    }

    public class UserMessage
    {
        public string Token { get; set; }
        public string Message { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FusionInsite.App.Server.Data.Models;

namespace FusionInsite.App.Server
{
    public interface IPushNotificationSender
    {
        PushResult Send(UserMessage message);
    }

    public class PushNotificationSender : IPushNotificationSender
    {
        public PushResult Send(UserMessage message)
        {
            throw new NotImplementedException();
        }
        
    }
}

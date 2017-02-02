using System.Collections.Generic;
using FusionInsite.App.Server.Data.Models;

namespace FusionInsite.App.Server.Data.Repositories.Interfaces
{
    public interface IUserSubscriptionRepository
    {
        List<string> GetUserTokensSubscribedToProtocol(int? protocolId, PushNotificationType notificationTypeId);
    }
}

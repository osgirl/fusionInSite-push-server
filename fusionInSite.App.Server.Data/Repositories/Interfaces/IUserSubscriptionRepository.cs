using System.Collections.Generic;

namespace FusionInsite.App.Server.Data.Repositories.Interfaces
{
    public interface IUserSubscriptionRepository
    {
        List<string> GetUsersSubscribedToProtocol(int protocolId);
    }
}
